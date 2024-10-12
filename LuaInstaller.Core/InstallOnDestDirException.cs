using System;

namespace LuaInstaller.Core
{
    public class InstallOnDestDirException : Exception
    {
        public InstallOnDestDirException()
        {
        }

        public InstallOnDestDirException(string message) : base(message)
        {
        }

        public InstallOnDestDirException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
