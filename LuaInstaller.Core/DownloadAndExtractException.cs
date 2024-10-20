using System;

namespace LuaInstaller.Core
{
    public class DownloadAndExtractException : Exception
    {
        public DownloadAndExtractException()
        {
        }

        public DownloadAndExtractException(string message) : base(message)
        {
        }

        public DownloadAndExtractException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
