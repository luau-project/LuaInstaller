using System;
using System.IO;

namespace LuaInstaller.Core
{
    public class PkgConfigOutputInfo
    {
        private static readonly LuaVersion LMOD_CMOD_VERSION_CHANGE;

        static PkgConfigOutputInfo()
        {
            LMOD_CMOD_VERSION_CHANGE = new LuaVersion(5, 2, 4);
        }

        private readonly LuaVersion _version;
        private readonly LuaGeneratedBinaries _generatedBinaries;
        private readonly LuaDestinationDirectory _destinationDir;


        public PkgConfigOutputInfo(LuaVersion version, LuaGeneratedBinaries generatedBinaries, LuaDestinationDirectory luaDestinationDirectory)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (generatedBinaries == null)
            {
                throw new ArgumentNullException("generatedBinaries");
            }

            if (luaDestinationDirectory == null)
            {
                throw new ArgumentNullException("luaDestinationDirectory");
            }

            _version = version;
            _generatedBinaries = generatedBinaries;
            _destinationDir = luaDestinationDirectory;
        }

        public string Prefix
        { 
            get
            {
                return _destinationDir.Path;
            }
        }

        public string ExecPrefix
        {
            get
            {
                return Prefix;
            }
        }

        public string LibName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(_generatedBinaries.ImportLibName);
            }
        }

        public string LibDir
        {
            get
            {
                return _destinationDir.Lib;
            }
        }

        public string IncludeDir
        {
            get
            {
                return _destinationDir.Include;
            }
        }

        public string PkgConfigDir
        {
            get
            {
                return Path.Combine(_destinationDir.Lib, "pkgconfig");
            }
        }

        public string Version
        {
            get
            {
                return _version.Version;
            }
        }

        public string VersionShort
        {
            get
            {
                return _version.ShortVersion;
            }
        }

        public string InstallBin
        {
            get
            {
                return _destinationDir.Bin;
            }
        }

        public string InstallLib
        {
            get
            {
                return _destinationDir.Lib;
            }
        }

        public string InstallInc
        {
            get
            {
                return _destinationDir.Include;
            }
        }

        public string InstallDoc
        {
            get
            {
                return _destinationDir.Doc;
            }
        }

        public string InstallLuaModules
        {
            get
            {
                return LuaVersionComparers.Ascending.Compare(_version, LMOD_CMOD_VERSION_CHANGE) > 0 ?
                    Path.Combine(_destinationDir.Path, "share", "lua", _version.ShortVersion) :
                    Path.Combine(_destinationDir.Bin, "lua");
            }
        }

        public string InstallCModules
        {
            get
            {
                return LuaVersionComparers.Ascending.Compare(_version, LMOD_CMOD_VERSION_CHANGE) > 0 ?
                    Path.Combine(_destinationDir.Lib, "lua", _version.ShortVersion):
                    _destinationDir.Bin;
            }
        }

        public void Write(Stream stream)
        {
            using (TextWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(string.Format("prefix={0}", Prefix.Replace('\\', '/')));
                writer.WriteLine(string.Format("exec_prefix={0}", ExecPrefix.Replace('\\', '/')));
                writer.WriteLine(string.Format("lib_name={0}", LibName));
                writer.WriteLine(string.Format("libdir={0}", LibDir.Replace('\\', '/')));
                writer.WriteLine(string.Format("includedir={0}", IncludeDir.Replace('\\', '/')));
                writer.WriteLine();
                writer.WriteLine(string.Format("V={0}", VersionShort));
                writer.WriteLine(string.Format("R={0}", Version));
                writer.WriteLine(string.Format("INSTALL_BIN={0}", InstallBin.Replace('\\', '/')));
                writer.WriteLine(string.Format("INSTALL_LIB={0}", InstallLib.Replace('\\', '/')));
                writer.WriteLine(string.Format("INSTALL_INC={0}", InstallInc.Replace('\\', '/')));
                writer.WriteLine(string.Format("INSTALL_LMOD={0}", InstallLuaModules.Replace('\\', '/')));
                writer.WriteLine(string.Format("INSTALL_CMOD={0}", InstallCModules.Replace('\\', '/')));
                writer.WriteLine();
                writer.WriteLine("Name: Lua");
                writer.WriteLine("Description: Lua language engine");
                writer.WriteLine("Version: ${R}");
                writer.WriteLine("Requires:");
                writer.WriteLine("Libs: -L${libdir} -l${lib_name}");
                writer.WriteLine("Cflags: -I${includedir}");
                writer.WriteLine();
            }
        }

        public bool WritePkgConfigFile()
        {
            bool success = false;

            string dir = PkgConfigDir;

            DirectoryInfo pkgConfigDir = Directory.CreateDirectory(dir);

            if (success = pkgConfigDir.Exists)
            {
                string pkgConfigFilename = string.Format("lua{0}.pc", _version.ShortVersionWithoutDot);
                
                using (Stream luapc = File.OpenWrite(Path.Combine(dir, pkgConfigFilename)))
                {
                    Write(luapc);
                }
            }

            return success;
        }
    }
}
