using System;

namespace LuaInstaller.Core
{
    public class CompileCompilerException : BuildCompilerException
    {
        public CompileCompilerException()
        {
        }

        public CompileCompilerException(string message) : base(message)
        {
        }

        public CompileCompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
