using System;

namespace LuaInstaller.Core
{
    public class LinkDllException : BuildDllException
    {
        public LinkDllException()
        {
        }

        public LinkDllException(string message) : base(message)
        {
        }

        public LinkDllException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
