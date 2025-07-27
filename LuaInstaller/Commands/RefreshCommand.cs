using LuaInstaller.Core;
using LuaInstaller.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace LuaInstaller.Commands
{
    public class RefreshCommand : ICommand
    {
        private bool isRefreshing;
        private bool canRefresh;

        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            private set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    FireRefreshing();
                }
            }
        }

        private void FireRefreshing()
        {
            OnCanExecutedChanged();
            OnPropertyChanged("IsRefreshing");
        }

        public event EventHandler CanExecuteChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnCanExecutedChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool CanExecute(object parameter)
        {
            LuaInstallerViewModel viewModel = (LuaInstallerViewModel)parameter;
            bool canRefreshCurrently = !IsRefreshing && !viewModel.InstallCommand.IsInstalling;
            if (canRefresh != canRefreshCurrently)
            {
                canRefresh = canRefreshCurrently;
                OnCanExecutedChanged();
                viewModel.UpdateCanInstall();
            }
            return canRefresh;
        }

        public void Execute(object parameter)
        {
            IsRefreshing = true;

            BackgroundWorker workerThread = new BackgroundWorker();
            workerThread.DoWork += WorkerThread_DoWork;
            workerThread.RunWorkerCompleted += WorkerThread_RunWorkerCompleted;

            LuaInstallerViewModel viewModel = (LuaInstallerViewModel)parameter;
            viewModel.Status = "Refreshing versions";

            workerThread.RunWorkerAsync(parameter);
        }

        private class WorkerThreadResult
        {
            public LuaVersion[] Versions { get; set; }
            public LuaInstallerViewModel ViewModel { get; set; }
        }

        private void WorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                WorkerThreadResult result = ((WorkerThreadResult)(e.Result));

                LuaVersion[] versions = result.Versions;
                LuaInstallerViewModel viewModel = result.ViewModel;

                viewModel.LuaVersions.Clear();
                foreach (LuaVersion v in versions)
                {
                    viewModel.LuaVersions.Add(v);
                }

                if (viewModel.LuaVersions.Count > 0)
                {
                    viewModel.SelectedLuaVersion = viewModel.LuaVersions[0];
                }
                else
                {
                    viewModel.SelectedLuaVersion = null;
                }

                viewModel.Status = "Versions refreshed";
            }
            else
            {
                RefreshException ex = (RefreshException)(e.Error);

                LuaInstallerViewModel viewModel = ex.ViewModel;
                viewModel.Status = "Failed to obtain versions from website";
            }

            IsRefreshing = false;
        }

        private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = new WorkerThreadResult
                {
                    Versions = LuaWebsite.QueryVersions(),
                    ViewModel = (LuaInstallerViewModel)(e.Argument)
                };
            }
            catch (Exception ex)
            {
                throw new RefreshException((LuaInstallerViewModel)(e.Argument), ex);
            }
        }
    }
}
