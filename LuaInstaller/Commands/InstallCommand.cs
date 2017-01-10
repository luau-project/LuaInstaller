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
            return !IsInstalling && !viewModel.RefreshCommand.IsRefreshing && viewModel.DestinationDir != null && viewModel.SelectedLuaVersion != null && viewModel.SelectedLuaVersion != null && viewModel.SelectedWinSdkVersion != null;
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
                    case InstallationProgress.Download:
                        {
                            viewModel.Status = "Unable to create destination directory or download sources.";
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

                manager.Build(viewModel.SelectedLuaVersion, viewModel.DestinationDir, viewModel.SelectedVisualStudioVersion, viewModel.SelectedWinSdkVersion, variableTarget);
                e.Result = viewModel;
            }
            catch (Exception ex)
            {
                throw new InstallationException(viewModel, ex);
            }
        }
    }
}
