using System;

namespace LuaInstaller.Core
{
    public class BuildDllException : Exception
    {
        public BuildDllException()
        {
        }

        public BuildDllException(string message) : base(message)
        {
        }

        public BuildDllException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
