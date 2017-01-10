using System;

namespace LuaInstaller.Core
{
    public class BuildInterpreterException : Exception
    {
        public BuildInterpreterException()
        {
        }

        public BuildInterpreterException(string message) : base(message)
        {
        }

        public BuildInterpreterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
