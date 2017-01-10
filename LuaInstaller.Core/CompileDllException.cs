using System;

namespace LuaInstaller.Core
{
    public class CompileDllException : BuildDllException
    {
        public CompileDllException()
        {
        }

        public CompileDllException(string message) : base(message)
        {
        }

        public CompileDllException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
