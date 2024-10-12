using System;

namespace LuaInstaller.Core
{
    public class CreatePkgConfigFileException : Exception
    {
        public CreatePkgConfigFileException()
        {
        }

        public CreatePkgConfigFileException(string message) : base(message)
        {
        }

        public CreatePkgConfigFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
