using System;

namespace LuaInstaller.Core
{
    public class LinkCompilerException : BuildCompilerException
    {
        public LinkCompilerException()
        {
        }

        public LinkCompilerException(string message) : base(message)
        {
        }

        public LinkCompilerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
