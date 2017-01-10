using System;

namespace LuaInstaller.Core
{
    public class CompileInterpreterException : BuildInterpreterException
    {
        public CompileInterpreterException()
        {
        }

        public CompileInterpreterException(string message) : base(message)
        {
        }

        public CompileInterpreterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
