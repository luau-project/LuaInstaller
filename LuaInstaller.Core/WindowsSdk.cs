using System;

namespace LuaInstaller.Core
{
    public class WindowsSdk
    {
        private readonly WindowsSdkVersion version;
        private readonly Architecture arch;
        private readonly IncludeDirectories includeDirectories;
        private readonly LibPathDirectories libDirectories;

        public WindowsSdk(WindowsSdkVersion version, Architecture arch, IncludeDirectories includeDirectories, LibPathDirectories libDirectories)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (includeDirectories == null)
            {
                throw new ArgumentNullException("includeDirectories");
            }

            if (libDirectories == null)
            {
                throw new ArgumentNullException("libDirectories");
            }

            this.version = version;
            this.arch = arch;
            this.includeDirectories = includeDirectories;
            this.libDirectories = libDirectories;
        }

        public Architecture Arch { get { return arch; } }
        public WindowsSdkVersion Version { get { return version; } }
        public IncludeDirectories IncludeDirectories { get { return includeDirectories; } }
        public LibPathDirectories LibPathDirectories { get { return libDirectories; } }
    }
}
