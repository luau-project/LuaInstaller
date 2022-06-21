using LuaInstaller.Commands;
using LuaInstaller.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace LuaInstaller.ViewModels
{
    public class LuaInstallerViewModel : INotifyPropertyChanged
    {
        private readonly InstalledComponents components;

        private Architecture platform;
        private string destinationDir;
        private ObservableCollection<LuaVersion> luaVersions;
        private ObservableCollection<VisualStudio> visualStudioVersions;
        private ObservableCollection<WindowsSdk> winSdkVersions;
        private bool setEnvironmentVariables;
        private EnvironmentVariableTarget variableTarget;

        private LuaVersion selectedLuaVersion;
        private VisualStudio selectedVisualStudioVersion;
        private WindowsSdk selectedWinSdkVersion;

        private readonly RefreshCommand refreshCommand;
        private readonly InstallCommand installCommand;

        private string status;
        private InstallationProgress progress;

        public Architecture Platform
        {
            get
            {
                return platform;
            }

            set
            {
                if (platform != value)
                {
                    platform = value;

                    ChangePlatform();
                    OnPropertyChanged("Platform");
                }
            }
        }

        public string DestinationDir
        {
            get
            {
                return destinationDir;
            }

            set
            {
                if (destinationDir != null)
                {
                    destinationDir = value;
                    OnPropertyChanged("DestinationDir");
                }
            }
        }

        public ObservableCollection<LuaVersion> LuaVersions
        {
            get
            {
                return luaVersions;
            }

            set
            {
                if (luaVersions != value)
                {
                    luaVersions = value;
                    OnPropertyChanged("LuaVersions");
                }
            }
        }

        public ObservableCollection<VisualStudio> VisualStudioVersions
        {
            get
            {
                return visualStudioVersions;
            }

            set
            {
                if (visualStudioVersions != value)
                {
                    visualStudioVersions = value;
                    OnPropertyChanged("VisualStudioVersions");
                }
            }
        }

        public ObservableCollection<WindowsSdk> WinSdkVersions
        {
            get
            {
                return winSdkVersions;
            }

            set
            {
                if (winSdkVersions == value)
                {
                    winSdkVersions = value;
                    OnPropertyChanged("WinSdkVersions");
                }
            }
        }

        public LuaVersion SelectedLuaVersion
        {
            get
            {
                return selectedLuaVersion;
            }

            set
            {
                if (selectedLuaVersion != value)
                {
                    selectedLuaVersion = value;
                    OnPropertyChanged("SelectedLuaVersion");
                }
            }
        }

        public VisualStudio SelectedVisualStudioVersion
        {
            get
            {
                return selectedVisualStudioVersion;
            }

            set
            {
                if (selectedVisualStudioVersion != value)
                {
                    selectedVisualStudioVersion = value;
                    OnPropertyChanged("SelectedVisualStudioVersion");
                }
            }
        }

        public WindowsSdk SelectedWinSdkVersion
        {
            get
            {
                return selectedWinSdkVersion;
            }

            set
            {
                if (selectedWinSdkVersion != value)
                {
                    selectedWinSdkVersion = value;
                    OnPropertyChanged("SelectedWinSdkVersion");
                }
            }
        }

        public RefreshCommand RefreshCommand
        {
            get
            {
                return refreshCommand;
            }
        }

        public InstalledComponents Components
        {
            get
            {
                return components;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public InstallCommand InstallCommand
        {
            get
            {
                return installCommand;
            }
        }

        public InstallationProgress Progress
        {
            get
            {
                return progress;
            }

            set
            {
                if (progress != value)
                {
                    progress = value;
                    OnPropertyChanged("Progress");
                }
            }
        }

        public bool SetEnvironmentVariables
        {
            get
            {
                return setEnvironmentVariables;
            }

            set
            {
                if (setEnvironmentVariables != value)
                {
                    setEnvironmentVariables = value;
                    OnPropertyChanged("SetEnvironmentVariables");
                }
            }
        }

        public EnvironmentVariableTarget VariableTarget
        {
            get
            {
                return variableTarget;
            }

            set
            {
                if (value != EnvironmentVariableTarget.User && value != EnvironmentVariableTarget.Machine)
                {
                    throw new Exception("Invalid variable target");
                }

                if (variableTarget != value)
                {
                    variableTarget = value;
                    OnPropertyChanged("VariableTarget");
                }
            }
        }

        public LuaInstallerViewModel()
        {
            components = new InstalledComponents();

            refreshCommand = new RefreshCommand();
            installCommand = new InstallCommand();

            status = string.Empty;
            progress = InstallationProgress.None;
            setEnvironmentVariables = false;
            variableTarget = EnvironmentVariableTarget.User;

            destinationDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".lua"
            );

            luaVersions = new ObservableCollection<LuaVersion>(
                LuaWebsite.QueryVersions()
            );

            if (luaVersions.Count > 0)
            {
                selectedLuaVersion = luaVersions[0];
            }

            platform = Environment.Is64BitOperatingSystem ? Architecture.X64 : Architecture.X86;

            visualStudioVersions = new ObservableCollection<VisualStudio>(
                components.AllVisualStudioX86().ToArray()
            );

            if (visualStudioVersions.Count > 0)
            {
                selectedVisualStudioVersion = visualStudioVersions[0];
            }

            winSdkVersions = new ObservableCollection<WindowsSdk>(
                components.AllWindowsSdkX86().ToArray()
            );


            if (winSdkVersions.Count > 0)
            {
                selectedWinSdkVersion = winSdkVersions[0];
            }
        }

        private void ChangePlatform()
        {
            switch (platform)
            {
                case Architecture.X86:
                    {
                        visualStudioVersions.Clear();
                        VisualStudio[] vsList = components.AllVisualStudioX86().ToArray();
                        foreach (VisualStudio vs in vsList)
                        {
                            visualStudioVersions.Add(vs);
                        }
                        
                        if (visualStudioVersions.Count > 0)
                        {
                            SelectedVisualStudioVersion = visualStudioVersions[0];
                        }
                        else
                        {
                            SelectedVisualStudioVersion = null;
                        }

                        winSdkVersions.Clear();
                        WindowsSdk[] winSdkList = components.AllWindowsSdkX86().ToArray();
                        foreach (WindowsSdk sdk in winSdkList)
                        {
                            winSdkVersions.Add(sdk);
                        }

                        if (winSdkVersions.Count > 0)
                        {
                            SelectedWinSdkVersion = winSdkVersions[0];
                        }
                        else
                        {
                            SelectedWinSdkVersion = null;
                        }
                    }
                    break;
                case Architecture.X64:
                    {
                        visualStudioVersions.Clear();
                        VisualStudio[] vsList = components.AllVisualStudioX64().ToArray();
                        foreach (VisualStudio vs in vsList)
                        {
                            visualStudioVersions.Add(vs);
                        }

                        if (visualStudioVersions.Count > 0)
                        {
                            SelectedVisualStudioVersion = visualStudioVersions[0];
                        }
                        else
                        {
                            SelectedVisualStudioVersion = null;
                        }

                        winSdkVersions.Clear();
                        WindowsSdk[] winSdkList = components.AllWindowsSdkX64().ToArray();
                        foreach (WindowsSdk sdk in winSdkList)
                        {
                            winSdkVersions.Add(sdk);
                        }

                        if (winSdkVersions.Count > 0)
                        {
                            SelectedWinSdkVersion = winSdkVersions[0];
                        }
                        else
                        {
                            SelectedWinSdkVersion = null;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
