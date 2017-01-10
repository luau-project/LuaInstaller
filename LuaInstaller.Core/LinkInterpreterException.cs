using System;

namespace LuaInstaller.Core
{
    public class LinkInterpreterException : BuildInterpreterException
    {
        public LinkInterpreterException()
        {
        }

        public LinkInterpreterException(string message) : base(message)
        {
        }

        public LinkInterpreterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
