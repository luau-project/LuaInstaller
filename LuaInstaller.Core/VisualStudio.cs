using System;

namespace LuaInstaller.Core
{
    public class VisualStudio
    {
        private readonly VisualStudioVersion version;
        private readonly VisualStudioToolset toolset;
        private readonly Architecture arch;
        private readonly IncludeDirectories includeDirs;
        private readonly LibPathDirectories libDirs;

        public VisualStudio(VisualStudioVersion version, VisualStudioToolset toolset, Architecture arch, IncludeDirectories includeDirs, LibPathDirectories libDirs)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (toolset == null)
            {
                throw new ArgumentNullException("toolset");
            }

            if (includeDirs == null)
            {
                throw new ArgumentNullException("includeDirs");
            }

            if (libDirs == null)
            {
                throw new ArgumentNullException("libDirs");
            }

            this.version = version;
            this.toolset = toolset;
            this.arch = arch;
            this.includeDirs = includeDirs;
            this.libDirs = libDirs;
        }

        public Architecture Arch { get { return arch; } }
        public VisualStudioToolset Toolset { get { return toolset; } }
        public VisualStudioVersion Version { get { return version; } }
        public IncludeDirectories IncludeDirectories { get { return includeDirs; } }
        public LibPathDirectories LibPathDirectories { get { return libDirs; } }
    }
}
