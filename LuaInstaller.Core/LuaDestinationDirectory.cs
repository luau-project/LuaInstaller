using System;
using System.IO;

namespace LuaInstaller.Core
{
    public class LuaDestinationDirectory
    {
        private readonly string _path;
        private readonly string _bin;
        private readonly string _doc;
        private readonly string _include;
        private readonly string _lib;

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string Bin
        {
            get
            {
                return _bin;
            }
        }

        public string Doc
        {
            get
            {
                return _doc;
            }
        }

        public string Include
        {
            get
            {
                return _include;
            }
        }

        public string Lib
        {
            get
            {
                return _lib;
            }
        }

        public LuaDestinationDirectory(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = System.IO.Path.GetFullPath(path);
            _bin = System.IO.Path.Combine(_path, "bin");
            _doc = System.IO.Path.Combine(_path, "doc");
            _include = System.IO.Path.Combine(_path, "include");
            _lib = System.IO.Path.Combine(_path, "lib");
        }

        public void CreateFrom(LuaSourcesDirectory sourcesDir)
        {
            string[] dirs = new string[]
            {
                _path,
                _bin,
                _doc,
                _include,
                _lib
            };

            foreach (string d in dirs)
            {
                Directory.CreateDirectory(d);
            }

            string[][] filesToCopy = new string[][]
            {
                new string[]
                {
                    System.IO.Path.Combine(sourcesDir.Path, "README"),
                    System.IO.Path.Combine(_path, "README"),
                },
                new string[]
                {
                    System.IO.Path.Combine(sourcesDir.Src, "lauxlib.h"),
                    System.IO.Path.Combine(_include, "lauxlib.h"),
                },
                new string[]
                {
                    System.IO.Path.Combine(sourcesDir.Src, "lua.h"),
                    System.IO.Path.Combine(_include, "lua.h"),
                },
                new string[]
                {
                    System.IO.Path.Combine(sourcesDir.Src, "lua.hpp"),
                    System.IO.Path.Combine(_include, "lua.hpp"),
                },
                new string[]
                {
                    System.IO.Path.Combine(sourcesDir.Src, "luaconf.h"),
                    System.IO.Path.Combine(_include, "luaconf.h"),
                },
                new string[]
                {
                    System.IO.Path.Combine(sourcesDir.Src, "lualib.h"),
                    System.IO.Path.Combine(_include, "lualib.h"),
                }
            };

            foreach (string[] files in filesToCopy)
            {
                string fromPath = files[0];
                string toPath = files[1];

                if (File.Exists(fromPath))
                {
                    File.Copy(fromPath, toPath, true);
                }
            }

            DirectoryInfo docInfo = new DirectoryInfo(sourcesDir.Doc);
            foreach (FileInfo fi in docInfo.GetFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(_doc, fi.Name), true);
            }
        }
    }
}
