using LuaInstaller.ViewModels;
using System;

namespace LuaInstaller.Commands
{
    public class InstallationException : Exception
    {
        private readonly LuaInstallerViewModel viewModel;

        public InstallationException(LuaInstallerViewModel viewModel, Exception innerException)
            : this(viewModel, null, innerException)
        {
            this.viewModel = viewModel;
        }
        public InstallationException(LuaInstallerViewModel viewModel, string message, Exception innerException)
            : base(message, innerException)
        {
            this.viewModel = viewModel;
        }

        public LuaInstallerViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
        }
    }
}
