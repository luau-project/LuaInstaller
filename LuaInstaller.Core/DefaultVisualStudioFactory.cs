using System;
using System.IO;

namespace LuaInstaller.Core
{
    public class DefaultVisualStudioFactory : IVisualStudioFactory
    {
        public VisualStudio Create(VisualStudioVersion version, Architecture arch)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            VisualStudio result = null;

            if (arch == Architecture.X86)
            {
                string cl = Path.Combine(version.VcDir, "bin", "cl.exe");
                string link = Path.Combine(version.VcDir, "bin", "link.exe");
                string ml = Path.Combine(version.VcDir, "bin", "ml.exe");

                if (File.Exists(cl) && File.Exists(link) && File.Exists(ml))
                {
                    VisualStudioToolset toolset = new VisualStudioToolset(cl, link, ml);
                    
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
                string cl = Path.Combine(version.VcDir, "bin", "amd64", "cl.exe");
                string link = Path.Combine(version.VcDir, "bin", "amd64", "link.exe");
                string ml = Path.Combine(version.VcDir, "bin", "amd64", "ml64.exe");

                if (File.Exists(cl) && File.Exists(link) && File.Exists(ml))
                {
                    VisualStudioToolset toolset = new VisualStudioToolset(cl, link, ml);

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

            return result;
        }
    }
}
