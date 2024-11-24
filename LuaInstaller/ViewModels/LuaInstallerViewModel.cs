using LuaInstaller.Commands;
using LuaInstaller.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

        public string InstallerVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo assemblyInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return assemblyInfo.FileVersion;
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

            IVisualStudioEnumeration vsEnum = components.AllVisualStudioByArch(platform);

            visualStudioVersions = new ObservableCollection<VisualStudio>(
                vsEnum
            );

            vsEnum.TryGetLatest(out selectedVisualStudioVersion);

            IWindowsSdkEnumeration windowsSdkEnum = components.AllWindowsSdkByArch(platform);

            winSdkVersions = new ObservableCollection<WindowsSdk>(
                windowsSdkEnum
            );

            windowsSdkEnum.TryGetLatest(out selectedWinSdkVersion);
        }

        private void ChangePlatform()
        {
            visualStudioVersions.Clear();
            IVisualStudioEnumeration vsEnum = components.AllVisualStudioByArch(platform);
            foreach (VisualStudio vs in vsEnum)
            {
                visualStudioVersions.Add(vs);
            }

            VisualStudio latestVs;
            vsEnum.TryGetLatest(out latestVs);
            SelectedVisualStudioVersion = latestVs;

            winSdkVersions.Clear();
            IWindowsSdkEnumeration windowsSdksEnum = components.AllWindowsSdkByArch(platform);
            foreach (WindowsSdk sdk in windowsSdksEnum)
            {
                winSdkVersions.Add(sdk);
            }

            WindowsSdk latestSdk;
            windowsSdksEnum.TryGetLatest(out latestSdk);
            SelectedWinSdkVersion = latestSdk;
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
