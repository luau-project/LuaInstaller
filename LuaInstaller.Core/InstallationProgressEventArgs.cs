using System;

namespace LuaInstaller.Core
{
    public class InstallationProgressEventArgs: EventArgs
    {
        private readonly InstallationProgress progress;

        public InstallationProgressEventArgs(InstallationProgress progress)
            : base()
        {
            this.progress = progress;
        }

        public InstallationProgress Progress
        {
            get
            {
                return progress;
            }
        }
    }
}
