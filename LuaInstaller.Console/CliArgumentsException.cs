using System;

namespace LuaInstaller.Console
{
    public class CliArgumentsException : Exception
    {
        public CliArgumentsException(string message) : base(message)
        {
        }

        public CliArgumentsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
