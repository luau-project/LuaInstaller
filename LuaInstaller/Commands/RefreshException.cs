using LuaInstaller.ViewModels;
using System;

namespace LuaInstaller.Commands
{
    public class RefreshException : Exception
    {
        private readonly LuaInstallerViewModel viewModel;

        public RefreshException(LuaInstallerViewModel viewModel, Exception innerException)
            : this(viewModel, null, innerException)
        {

        }

        public RefreshException(LuaInstallerViewModel viewModel, string message, Exception innerException) : base(message, innerException)
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
