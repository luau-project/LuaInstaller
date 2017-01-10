using System;

namespace LuaInstaller.Core
{
    public class EnvironmentVariablesException : Exception
    {
        public EnvironmentVariablesException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
