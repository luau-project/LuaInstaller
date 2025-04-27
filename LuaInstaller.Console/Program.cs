using LuaInstaller.Core;
using System;
using System.Diagnostics;
using System.Reflection;

namespace LuaInstaller.Console
{
    class Program
    {
        private static string InstallerVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo assemblyInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return assemblyInfo.FileVersion;
            }
        }

        private static string InstallerWebsite
        {
            get
            {
                return "https://github.com/luau-project/LuaInstaller";
            }
        }

        private static void Write(string msg)
        {
            System.Console.WriteLine(msg);
        }

        private static void Help()
        {
            Write(@"
------------------------------------------------------
Information
------------------------------------------------------

LuaInstaller.Console.exe [ /? | help | -h | --help ]
    Displays this help message

LuaInstaller.Console.exe [ -v | --version ]
    Displays the version of the installer

LuaInstaller.Console.exe help-website
    Displays the website of the installer

LuaInstaller.Console.exe list-lua
    Lists all Lua versions that this tool
    is able to build

LuaInstaller.Console.exe list-vs
    Lists all MSVC compilers found
    matching the architecture
    (x86, x64 or ARM64) of the
    Operating System

LuaInstaller.Console.exe list-vs-x86
    Lists all MSVC x86 toolset
    compilers found

LuaInstaller.Console.exe list-vs-x64
    Lists all MSVC x64 toolset
    compilers found

LuaInstaller.Console.exe list-vs-arm64
    Lists all MSVC ARM64 toolset
    compilers found

LuaInstaller.Console.exe list-win-sdk
    Lists all Windows SDK found
    matching the architecture
    (x86, x64 or ARM64) of the
    Operating System

LuaInstaller.Console.exe list-win-sdk-x86
    Lists all Windows SDK x86 found

LuaInstaller.Console.exe list-win-sdk-x64
    Lists all Windows SDK x64 found

LuaInstaller.Console.exe list-win-sdk-arm64
    Lists all Windows SDK ARM64 found

------------------------------------------------------
Installation
------------------------------------------------------

LuaInstaller.Console.exe install { OPTION=VALUE }

    dest-dir=C:\test
        Destination directory to install
        e.g.: installs Lua on C:\test
        Defaults to the current directory.
        
    version=5.1.5
        Lua version to install
        e.g.: installs Lua 5.1.5

    arch=[ x86 | X86 | x64 | X64 | arm64 | ARM64 ]
        Generates machine code for the
        specified platform
        Defaults to the architecture
        (x86, x64 or ARM64) of the
        Operating System.

    vs=14.0
        Uses a specific version of the
        Microsoft Visual Studio toolset
        to build Lua
        e.g.: Uses VS 14.0 (VS 2015)

    win-sdk=10.0
        Uses a specific version of the
        Windows SDK
        e.g.: Uses Win SDK for Windows 10

    env-var=[ process | user | machine ]
        Sets environment variables
        for the specified target.
        Remark: if this switch is not
        specified, environment variables
        are not set at all.

------------------------------------------------------
Examples
------------------------------------------------------

1) Installs the latest Lua available on Lua's website
in the current directory, using the latest versions
of Visual Studio and Windows SDK, building the
source code matching the architecture (x86, x64 or ARM64)
of the Operating System:

    LuaInstaller.Console.exe install

2) Installs Lua 5.4.7 in the current directory,
using the latest versions of Visual Studio and
Windows SDK, building the source code for x64
platforms:

    LuaInstaller.Console.exe install version=5.4.7 arch=x64

3) Installs Lua 5.1.5 in the folder
'C:\Program Files (x86)\Lua',
using the latest versions of Visual Studio and
Windows SDK, building the source code matching
the architecture (x86, x64 or ARM64) of the
Operating System. Also sets environment variables
machine-wide.

------------------------------------------------------
Remark: This kind of machine-wide installation usually
requires 'Administrator' privileges, so you must
'Run As Administrator'
------------------------------------------------------

    LuaInstaller.Console.exe install ""dest-dir=C:\Program Files (x86)\Lua"" version=5.1.5 env-var=machine
");
        }
        
        private static int Install(string[] args, out InstallArguments installArgs)
        {
            int result = 0;
            installArgs = new InstallArguments(new InstalledComponents());
            
            try
            {
                installArgs.Process(args, 1);

                ICompiler compiler = new VisualStudioCompiler(installArgs.Vs.Toolset.Cl);
                ILinker linker = new VisualStudioLinker(installArgs.Vs.Toolset.Link);

                InstallationManager manager = new InstallationManager(compiler, linker);

                manager.ExecuteInstall(installArgs.Version, installArgs.OutDir, installArgs.Vs, installArgs.Winsdk, installArgs.VariableTarget);
            }
            catch (CliArgumentsException ex)
            {
                Write("Argument error: " + ex.Message);
                installArgs = null;
                result = 3;
            }
            catch (Exception ex)
            {
                Write(ex.Message);
                installArgs = null;
                result = 2;
            }

            return result;
        }

        private static int Main(string[] args)
        {
            int result = 0;
            int nargs = args.Length;
            bool installed = false;
            InstallArguments installArgs = null;

            if (nargs == 0)
            {
                Help();
            }
            else if (nargs == 1)
            {
                string action = args[0];

                switch (action)
                {
                    case "/?":
                    case "help":
                    case "-h":
                    case "--help":
                        {
                            Help();

                            break;
                        }
                    case "help-website":
                        {
                            Write(InstallerWebsite);
                            break;
                        }
                    case "-v":
                    case "--version":
                        {
                            Write(InstallerVersion);
                            break;
                        }
                    case "list-lua":
                        {
                            foreach (LuaVersion v in LuaWebsite.QueryVersions())
                            {
                                Write(v.Version);
                            }
                            break;
                        }
                    case "list-vs":
                        {
                            IInstalledComponents components = new InstalledComponents();
                            Architecture arch = ArchitectureSelector.Instance.Architecture;

                            foreach (VisualStudio vs in components.AllVisualStudioByArch(arch))
                            {
                                Write(vs.Version.ToString());
                            }
                            break;
                        }
                    case "list-vs-x86":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (VisualStudio vs in components.AllVisualStudioX86())
                            {
                                Write(vs.Version.ToString());
                            }
                            break;
                        }
                    case "list-vs-x64":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (VisualStudio vs in components.AllVisualStudioX64())
                            {
                                Write(vs.Version.ToString());
                            }
                            break;
                        }
                    case "list-vs-arm64":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (VisualStudio vs in components.AllVisualStudioARM64())
                            {
                                Write(vs.Version.ToString());
                            }
                            break;
                        }
                    case "list-win-sdk":
                        {
                            IInstalledComponents components = new InstalledComponents();
                            Architecture arch = ArchitectureSelector.Instance.Architecture;

                            foreach (WindowsSdk sdk in components.AllWindowsSdkByArch(arch))
                            {
                                Write(sdk.Version.ToString());
                            }
                            break;
                        }
                    case "list-win-sdk-x86":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (WindowsSdk sdk in components.AllWindowsSdkX86())
                            {
                                Write(sdk.Version.ToString());
                            }
                            break;
                        }
                    case "list-win-sdk-x64":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (WindowsSdk sdk in components.AllWindowsSdkX64())
                            {
                                Write(sdk.Version.ToString());
                            }
                            break;
                        }
                    case "list-win-sdk-arm64":
                        {
                            IInstalledComponents components = new InstalledComponents();

                            foreach (WindowsSdk sdk in components.AllWindowsSdkARM64())
                            {
                                Write(sdk.Version.ToString());
                            }
                            break;
                        }
                    case "install":
                        {
                            result = Install(args, out installArgs);
                            installed = result == 0;
                            break;
                        }
                    default:
                        {
                            Write("Unknown option");
                            result = 1;
                            break;
                        }
                }
            }
            else
            {
                string action = args[0];

                switch (action)
                {
                    case "install":
                        {
                            result = Install(args, out installArgs);
                            installed = result == 0;
                            break;
                        }
                    default:
                        {
                            Write("Unknown option");
                            result = 1;
                        }
                        break;
                }
            }
            
            if (installed)
            {
                Write("Lua was installed successfully:");
                Write(string.Format("  Lua version: {0}", installArgs.Version.Version));
                Write(string.Format("  Architecture: {0}", installArgs.Arch.ToString()));
                Write(string.Format("  Destination: {0}", installArgs.OutDir));
                Write(string.Format("  Visual Studio: {0}", installArgs.Vs.Version.ToString()));
                Write(string.Format("  Windows SDK: {0}", installArgs.Winsdk.Version.ToString()));
                Write("LuaInstaller.Console:");
                Write(string.Format("  Version: {0}", InstallerVersion));
                Write(string.Format("  Website: {0}", InstallerWebsite));
            }

            return result;
        }
    }
}
