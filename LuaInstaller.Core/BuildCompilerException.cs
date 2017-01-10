using System;

namespace LuaInstaller.Core
{
    public class BuildCompilerException : Exception
    {
        public BuildCompilerException()
        {
        }

        public BuildCompilerException(string message) : base(message)
        {
        }

        public BuildCompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
