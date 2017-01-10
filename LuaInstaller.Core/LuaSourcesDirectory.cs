using System;

namespace LuaInstaller.Core
{
    public class LuaSourcesDirectory
    {
        private readonly string _path;
        private readonly string _doc;
        private readonly string _src;

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

        public LuaSourcesDirectory(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;
            _doc = System.IO.Path.Combine(_path, "doc");
            _src = System.IO.Path.Combine(_path, "src");
        }
    }
}
