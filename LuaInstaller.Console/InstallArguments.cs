using LuaInstaller.Core;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuaInstaller.Console
{
    public class InstallArguments : ICliArguments
    {
        private IInstalledComponents components;

        private LuaVersion version;
        private Architecture arch;
        private string outDir;
        private VisualStudio vs;
        private WindowsSdk winsdk;
        private EnvironmentVariableTarget? variableTarget;

        public InstallArguments(IInstalledComponents components)
        {
            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            this.components = components;
        }

        public Architecture Arch
        {
            get
            {
                return arch;
            }
        }

        public string OutDir
        {
            get
            {
                return outDir;
            }
        }

        public VisualStudio Vs
        {
            get
            {
                return vs;
            }
        }

        public WindowsSdk Winsdk
        {
            get
            {
                return winsdk;
            }
        }

        public LuaVersion Version
        {
            get
            {
                return version;
            }
        }

        public EnvironmentVariableTarget? VariableTarget
        {
            get
            {
                return variableTarget;
            }
        }

        public void Process(string[] args, int index)
        {
            Regex keyValueRgx = new Regex(@"^([^\=]+)\=(.+)$");
            Match keyValueMatch = null;

            Regex majorMinorRgx = new Regex(@"^(\d+)\.(\d+)$");
            
            string archArg = null;
            string vsVer = null;
            string winsdkVer = null;

            int nargs = args.Length;
            int i = index;

            while (i < nargs)
            {
                keyValueMatch = keyValueRgx.Match(args[i]);

                if (keyValueMatch.Success)
                {
                    string key = keyValueMatch.Groups[1].Value;
                    string value = keyValueMatch.Groups[2].Value;

                    switch (key)
                    {
                        case "version":
                            {
                                if (version != null)
                                {
                                    throw new InvalidOptionException("Lua version set multiple times");
                                }

                                try
                                {
                                    version = LuaWebsite.FindVersion(value);
                                }
                                catch
                                {
                                    throw new InvalidOptionException("Lua version not found");
                                }
                                break;
                            }
                        case "arch":
                            {
                                if (archArg != null)
                                {
                                    throw new InvalidOptionException("Architecture set multiple times");
                                }

                                archArg = value;
                                break;
                            }
                        case "dest-dir":
                            {
                                if (outDir != null)
                                {
                                    throw new InvalidOptionException("Destination directory set multiple times");
                                }

                                try
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(value);
                                    outDir = di.FullName;
                                }
                                catch
                                {
                                    throw new InvalidOptionValueException("Unable to create destination directory");
                                }

                                break;
                            }
                        case "vs":
                            {
                                if (vsVer != null)
                                {
                                    throw new InvalidOptionException("Visual Studio set multiple times");
                                }

                                vsVer = value;
                                break;
                            }
                        case "win-sdk":
                            {
                                if (winsdkVer != null)
                                {
                                    throw new InvalidOptionException("Windows SDK set multiple times");
                                }

                                winsdkVer = value;
                                break;
                            }
                        case "env-var":
                            {
                                if (variableTarget.HasValue)
                                {
                                    throw new InvalidOperationException("Environment Variable set multiple times");
                                }

                                if (value == "user")
                                {
                                    variableTarget = EnvironmentVariableTarget.User;
                                }
                                else if (value == "machine")
                                {
                                    variableTarget = EnvironmentVariableTarget.Machine;
                                }
                                else if (value == "process")
                                {
                                    variableTarget = EnvironmentVariableTarget.Process;
                                }
                                else
                                {
                                    throw new InvalidOptionValueException("Unknown environment variable");
                                }

                                break;
                            }
                        default:
                            {
                                throw new InvalidOptionException("Unknown option");
                            }
                    }

                    i++;
                }
                else
                {
                    throw new InvalidOptionException("Invalid option format");
                }
            }

            if (outDir == null)
            {
                outDir = Directory.GetCurrentDirectory();
            }

            if (archArg == null)
            {
                arch = Environment.Is64BitOperatingSystem ? Architecture.X64 : Architecture.X86;
            }
            else if (!Enum.TryParse(archArg.ToUpperInvariant(), out arch))
            {
                throw new InvalidOptionValueException("Unknown architecture");
            }

            switch (arch)
            {
                case Architecture.X86:
                case Architecture.X64:
                    {
                        if (vsVer == null)
                        {
                            components.AllVisualStudioByArch(arch).TryGetLatest(out vs);
                        }
                        else
                        {
                            Match vsVerMatch = majorMinorRgx.Match(vsVer);
                            if (vsVerMatch.Success)
                            {
                                GroupCollection groups = vsVerMatch.Groups;
                                int major = int.Parse(groups[1].Value);
                                int minor = int.Parse(groups[2].Value);

                                vs = components.AllVisualStudioByArch(arch).FirstOrDefault(v => v.Version.Major == major && v.Version.Minor == minor);
                            }
                            else
                            {
                                vs = components.AllVisualStudioByArch(arch).FirstOrDefault(v => v.Version.ToString() == vsVer);
                            }
                        }

                        if (winsdkVer == null)
                        {
                            components.AllWindowsSdkByArch(arch).TryGetLatest(out winsdk);
                        }
                        else
                        {
                            Match winSdkMatch = majorMinorRgx.Match(winsdkVer);
                            if (winSdkMatch.Success)
                            {
                                GroupCollection groups = winSdkMatch.Groups;
                                int major = int.Parse(groups[1].Value);
                                int minor = int.Parse(groups[2].Value);

                                winsdk = components.AllWindowsSdkByArch(arch).FirstOrDefault(v => v.Version.Major == major && v.Version.Minor == minor);
                            }
                            else
                            {
                                winsdk = components.AllWindowsSdkByArch(arch).FirstOrDefault(v => v.Version.ToString() == winsdkVer);
                            }
                        }
                    }
                    break;
                default:
                    {
                        throw new InvalidOptionValueException("Unknown architecture");
                    }
            }

            if (version == null && !LuaWebsite.TryGetLatestVersion(out version))
            {
                throw new CliArgumentsException("Unable to retrieve the latest version from the website");
            }

            if (vs == null)
            {
                throw new InvalidOptionValueException("Visual Studio not found");
            }

            if (winsdk == null)
            {
                throw new InvalidOptionValueException("Windows SDK not found");
            }
        }
    }
}
