using LuaInstaller.Core;
using LuaInstaller.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace LuaInstaller.Commands
{
    public class InstallCommand : ICommand, INotifyPropertyChanged
    {
        private bool isInstalling;
        private bool canExecute;
        
        public bool IsInstalling
        {
            get
            {
                return isInstalling;
            }
            private set
            {
                if (isInstalling != value)
                {
                    isInstalling = value;
                    FireInstalling();
                }
            }
        }

        public bool NotInstalling
        {
            get
            {
                return !isInstalling;
            }
        }

        private void FireInstalling()
        {
            OnCanExecutedChanged();
            OnPropertyChanged("IsInstalling");
            OnPropertyChanged("NotInstalling");
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
            bool canExecuteCurrently = !IsInstalling && 
                !viewModel.RefreshCommand.IsRefreshing &&
                viewModel.DestinationDir != null &&
                viewModel.SelectedLuaVersion != null &&
                viewModel.SelectedVisualStudioVersion != null &&
                viewModel.SelectedWinSdkVersion != null;

            if (canExecute != canExecuteCurrently)
            {
                canExecute = canExecuteCurrently;

                OnCanExecutedChanged();
            }

            return canExecute;
        }

        public void Execute(object parameter)
        {
            IsInstalling = true;

            LuaInstallerViewModel viewModel = (LuaInstallerViewModel)parameter;
            viewModel.Status = "Downloading sources...";

            BackgroundWorker workerThread = new BackgroundWorker();
            workerThread.WorkerReportsProgress = true;
            workerThread.DoWork += WorkerThread_DoWork;
            workerThread.RunWorkerCompleted += WorkerThread_RunWorkerCompleted;
            workerThread.ProgressChanged += WorkerThread_ProgressChanged;
            workerThread.RunWorkerAsync(parameter);
        }

        private void WorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            LuaInstallerViewModel viewModel = (LuaInstallerViewModel)(e.UserState);
            viewModel.Progress = (InstallationProgress)(e.ProgressPercentage);

            switch (viewModel.Progress)
            {
                case InstallationProgress.Download:
                    {
                        viewModel.Status = "Download finished";
                    }
                    break;
                case InstallationProgress.CompileDll:
                    {
                        viewModel.Status = "lua.dll compilation finished";
                    }
                    break;
                case InstallationProgress.LinkDll:
                    {
                        viewModel.Status = "lua.dll was built successfully and is ready to use";
                    }
                    break;
                case InstallationProgress.CompileInterpreter:
                    {
                        viewModel.Status = "lua.exe compilation finished";
                    }
                    break;
                case InstallationProgress.LinkInterpreter:
                    {
                        viewModel.Status = "lua.exe was built successfully and is ready to use";
                    }
                    break;
                case InstallationProgress.CompileCompiler:
                    {
                        viewModel.Status = "luac.exe compilation finished";
                    }
                    break;
                case InstallationProgress.LinkCompiler:
                    {
                        viewModel.Status = "luac.exe was built successfully and is ready to use";
                    }
                    break;
                case InstallationProgress.InstallOnDestDir:
                    {
                        viewModel.Status = "Failed to install on destination directory";
                    }
                    break;
                case InstallationProgress.CreatePkgConfigFile:
                    {
                        viewModel.Status = "Failed to create a pkg-config file";
                    }
                    break;
                case InstallationProgress.EnvironmentVariables:
                    {
                        viewModel.Status = "Environment variables were set";
                    }
                    break;
                case InstallationProgress.Finished:
                    {
                        viewModel.Status = "Lua was installed successfully";
                    }
                    break;
                default:
                    break;
            }
        }

        private void WorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                LuaInstallerViewModel viewModel = (LuaInstallerViewModel)(e.Result);
                
                viewModel.Progress = InstallationProgress.None;
                viewModel.Status = "Lua was installed successfully";
            }
            else
            {
                InstallationException ex = (InstallationException)(e.Error);
                LuaInstallerViewModel viewModel = ex.ViewModel;

                switch (viewModel.Progress)
                {
                    case InstallationProgress.None:
                        {
                            viewModel.Status = "Unable to create destination directory or download sources.";
                        }
                        break;
                    case InstallationProgress.Download:
                        {
                            viewModel.Status = "Failed to compile DLL.";
                        }
                        break;
                    case InstallationProgress.CompileDll:
                        {
                            viewModel.Status = "Failed to link DLL.";
                        }
                        break;
                    case InstallationProgress.LinkDll:
                        {
                            viewModel.Status = "Failed to compile interpreter.";
                        }
                        break;
                    case InstallationProgress.CompileInterpreter:
                        {
                            viewModel.Status = "Failed to link interpreter.";
                        }
                        break;
                    case InstallationProgress.LinkInterpreter:
                    case InstallationProgress.CompileCompiler:
                        {
                            viewModel.Status = "Interpreter was built successfully, but compiler failed.";
                        }
                        break;
                    case InstallationProgress.LinkCompiler:
                        {
                            viewModel.Status = "Failed to place artifacts on destination directory.";
                        }
                        break;
                    case InstallationProgress.InstallOnDestDir:
                        {
                            viewModel.Status = "Failed to create pkg-config file on destination folder.";
                        }
                        break;
                    case InstallationProgress.CreatePkgConfigFile:
                        {
                            viewModel.Status = "Everything was installed, but environment variables were not set.";
                        }
                        break;
                    default:
                        {
                            viewModel.Status = "Build failed.";
                        }
                        break;
                }

                viewModel.Progress = InstallationProgress.None;
            }

            IsInstalling = false;

            BackgroundWorker workerThread = (BackgroundWorker)sender;
            if (workerThread != null)
            {
                workerThread.Dispose();
            }
        }

        private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            LuaInstallerViewModel viewModel = (LuaInstallerViewModel)(e.Argument);

            ICompiler compiler = new VisualStudioCompiler(viewModel.SelectedVisualStudioVersion.Toolset.Cl);
            ILinker linker = new VisualStudioLinker(viewModel.SelectedVisualStudioVersion.Toolset.Link);

            InstallationManager manager = new InstallationManager(compiler, linker);

            manager.InstallationProgressChanged += (s, iea) =>
            {
                BackgroundWorker workerThread = (BackgroundWorker)sender;
                workerThread.ReportProgress((int)(iea.Progress), e.Argument);
            };

            try
            {
                EnvironmentVariableTarget? variableTarget = null;
                if (viewModel.SetEnvironmentVariables)
                {
                    variableTarget = viewModel.VariableTarget;
                }

                manager.ExecuteInstall(viewModel.SelectedLuaVersion, viewModel.DestinationDir, viewModel.SelectedVisualStudioVersion, viewModel.SelectedWinSdkVersion, variableTarget);
                e.Result = viewModel;
            }
            catch (Exception ex)
            {
                throw new InstallationException(viewModel, ex);
            }
        }
    }
}
