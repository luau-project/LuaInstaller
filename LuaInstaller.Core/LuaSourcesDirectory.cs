using System;

namespace LuaInstaller.Core
{
    public class LuaSourcesDirectory
    {
        private readonly string _path;
        private readonly string _doc;
        private readonly string _src;
        private readonly string _etc;
        private readonly LuaVersion _version;

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string Doc
        {
            get
            {
                return _doc;
            }
        }

        public string Src
        {
            get
            {
                return _src;
            }
        }

        public LuaVersion Version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// Collects all the public headers that are going to be installed
        /// </summary>
        /// <returns>A readonly array containing the source path on disk</returns>
        /// <exception cref="LuaPublicHeaderException">When an expected public file header is not found,
        /// this exception is thrown.</exception>
        public LuaInstaller.Core.ReadOnlyStringArray CollectPublicHeaders()
        {
            const int size = 5;
            string[] result = new string[size];
            string[] targetBasenames = new string[size]
            {
                "lua.h", "lualib.h", "lauxlib.h", "luaconf.h", "lua.hpp"
            };

            string basename;
            string expectedDir;
            string filePath;

            for (int i = 0; i < targetBasenames.Length; i++)
            {
                basename = targetBasenames[i];
                expectedDir = (_version.Major == 5 && _version.Minor == 1 && basename == "lua.hpp") ?
                    _etc : _src;
                filePath = System.IO.Path.Combine(expectedDir, basename);
                if (System.IO.File.Exists(filePath))
                {
                    result[i] = filePath;
                }
                else
                {
                    throw new LuaPublicHeaderException(basename, string.Format("The file ${0} was not found in the source code.", basename));
                }
            }

            return new LuaInstaller.Core.ReadOnlyStringArray(result);
        }

        public LuaSourcesDirectory(LuaVersion version, string path)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;
            _doc = System.IO.Path.Combine(_path, "doc");
            _src = System.IO.Path.Combine(_path, "src");
            _etc = System.IO.Path.Combine(_path, "etc");
            _version = version;
        }
    }
}
