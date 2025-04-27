using System;
using System.IO;

namespace LuaInstaller.Core
{
    public class DefaultVisualStudioFactory : IVisualStudioFactory
    {
        private VisualStudio BeforeVisualStudioSetupApi(VisualStudioVersion version, Architecture arch)
        {
            VisualStudio result = null;

            if (arch == Architecture.X86)
            {
                string toolsetBinDir = Path.Combine(version.VcDir, "bin");

                string cl = Path.Combine(toolsetBinDir, "cl.exe");
                string link = Path.Combine(toolsetBinDir, "link.exe");

                if (File.Exists(cl) && File.Exists(link))
                {
                    VisualStudioToolset toolset = new VisualStudioToolset(cl, link);
                    
                    string include = Path.Combine(version.VcDir, "include");
                    string lib = Path.Combine(version.VcDir, "lib");

                    if (Directory.Exists(include) && Directory.Exists(lib))
                    {
                        IncludeDirectories includeDirs = new IncludeDirectories(new string[1] { include });
                        LibPathDirectories libDirs = new LibPathDirectories(new string[1] { lib });

                        result = new VisualStudio(version, toolset, arch, includeDirs, libDirs);
                    }
                }
            }
            else if (arch == Architecture.X64)
            {
                string toolsetBinDir = Path.Combine(version.VcDir, "bin", "amd64");

                string cl = Path.Combine(toolsetBinDir, "cl.exe");
                string link = Path.Combine(toolsetBinDir, "link.exe");

                if (File.Exists(cl) && File.Exists(link))
                {
                    VisualStudioToolset toolset = new VisualStudioToolset(cl, link);

                    string include = Path.Combine(version.VcDir, "include");
                    string lib = Path.Combine(version.VcDir, "lib", "amd64");

                    if (Directory.Exists(include) && Directory.Exists(lib))
                    {
                        IncludeDirectories includeDirs = new IncludeDirectories(new string[1] { include });
                        LibPathDirectories libDirs = new LibPathDirectories(new string[1] { lib });

                        result = new VisualStudio(version, toolset, arch, includeDirs, libDirs);
                    }
                }
            }
            else if (arch == Architecture.ARM64)
            {
                string toolsetBinDir = Path.Combine(version.VcDir, "bin", "arm64");

                string cl = Path.Combine(toolsetBinDir, "cl.exe");
                string link = Path.Combine(toolsetBinDir, "link.exe");

                if (File.Exists(cl) && File.Exists(link))
                {
                    VisualStudioToolset toolset = new VisualStudioToolset(cl, link);

                    string include = Path.Combine(version.VcDir, "include");
                    string lib = Path.Combine(version.VcDir, "lib", "arm64");

                    if (Directory.Exists(include) && Directory.Exists(lib))
                    {
                        IncludeDirectories includeDirs = new IncludeDirectories(new string[1] { include });
                        LibPathDirectories libDirs = new LibPathDirectories(new string[1] { lib });

                        result = new VisualStudio(version, toolset, arch, includeDirs, libDirs);
                    }
                }
            }

            return result;
        }

        public VisualStudio AfterVisualStudioSetupApi(VisualStudioVersion version, Architecture arch)
        {
            VisualStudio result = null;

            string archStr = arch.ToString();

            string include = Path.Combine(version.VcDir, "include");
            string lib = Path.Combine(version.VcDir, "lib", archStr);

            if (Directory.Exists(include) && Directory.Exists(lib))
            {
                string toolsetDir = Path.Combine(version.VcDir, "bin", "Host" + archStr, archStr);
                string cl = Path.Combine(toolsetDir, "cl.exe");
                string link = Path.Combine(toolsetDir, "link.exe");

                if (File.Exists(cl) && File.Exists(link))
                {
                    VisualStudioToolset toolset = new VisualStudioToolset(cl, link);

                    IncludeDirectories includeDirs = new IncludeDirectories(new string[1] { include });
                    LibPathDirectories libDirs = new LibPathDirectories(new string[1] { lib });

                    result = new VisualStudio(version, toolset, arch, includeDirs, libDirs);
                }
            }

            return result;
        }

        public VisualStudio Create(VisualStudioVersion version, Architecture arch)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            return version.Major < 15 ? BeforeVisualStudioSetupApi(version, arch) : AfterVisualStudioSetupApi(version, arch);
        }
    }
}
