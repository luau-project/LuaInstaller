using System;

namespace LuaInstaller.Core
{
    public class LuaPublicHeaderException : Exception
    {
        private readonly string filename;

        public string FileName
        {
            get
            {
                return this.filename;
            }
        }

        public LuaPublicHeaderException(string filename) : base()
        {
            this.filename = filename;
        }

        public LuaPublicHeaderException(string filename, string message) : base(message)
        {
            this.filename = filename;
        }

        public LuaPublicHeaderException(string filename, string message, Exception innerException) : base(message, innerException)
        {
            this.filename = filename;
        }
    }
}
