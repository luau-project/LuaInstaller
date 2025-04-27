using System;
using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public class InstalledComponents : IInstalledComponents
    {
        private readonly IVisualStudioFactory _vsFactory;
        private readonly IWindowsSdkFactory _winsdkFactory;
        private readonly IUniversalCrtSdkFactory _universalCrtSdkFactory;
        private readonly IVisualStudioVersionLocator[] _visualStudioVersionLocators;
        private readonly IWindowsSdkVersionLocator[] _windowsSdkVersionLocators;
        private readonly IUniversalCrtSdkVersionLocator _universalCrtSdkVersionLocator;

        private readonly IDictionary<Architecture, Func<IVisualStudioEnumeration>> _vsDispatcher;
        private readonly IDictionary<Architecture, Func<IWindowsSdkEnumeration>> _winsdkDispatcher;

        public InstalledComponents(
            IVisualStudioFactory vsFactory,
            IWindowsSdkFactory winsdkFactory,
            IUniversalCrtSdkFactory universalCrtSdkFactory,
            IVisualStudioVersionLocator[] visualStudioVersionLocators,
            IWindowsSdkVersionLocator[] windowsSdkVersionLocators,
            IUniversalCrtSdkVersionLocator universalCrtSdkVersionLocator
        )
        {
            if (vsFactory == null)
            {
                throw new ArgumentNullException("vsFactory");
            }

            if (winsdkFactory == null)
            {
                throw new ArgumentNullException("winsdkFactory");
            }

            if (universalCrtSdkFactory == null)
            {
                throw new ArgumentNullException("universalCrtSdkFactory");
            }

            if (visualStudioVersionLocators == null)
            {
                throw new ArgumentNullException("visualStudioVersionLocators");
            }

            if (windowsSdkVersionLocators == null)
            {
                throw new ArgumentNullException("windowsSdkVersionLocators");
            }

            if (universalCrtSdkVersionLocator == null)
            {
                throw new ArgumentNullException("universalCrtSdkVersionLocator");
            }

            _vsFactory = vsFactory;
            _winsdkFactory = winsdkFactory;
            _universalCrtSdkFactory = universalCrtSdkFactory;

            _visualStudioVersionLocators = new IVisualStudioVersionLocator[visualStudioVersionLocators.Length];
            visualStudioVersionLocators.CopyTo(_visualStudioVersionLocators, 0);

            _windowsSdkVersionLocators = new IWindowsSdkVersionLocator[windowsSdkVersionLocators.Length];
            windowsSdkVersionLocators.CopyTo(_windowsSdkVersionLocators, 0);

            _universalCrtSdkVersionLocator = universalCrtSdkVersionLocator;

            _vsDispatcher = new Dictionary<Architecture, Func<IVisualStudioEnumeration>>
            {
                { Architecture.X86, () => AllVisualStudioX86() },
                { Architecture.X64, () => AllVisualStudioX64() },
                { Architecture.ARM64, () => AllVisualStudioARM64() }
            };

            _winsdkDispatcher = new Dictionary<Architecture, Func<IWindowsSdkEnumeration>>
            {
                { Architecture.X86, () => AllWindowsSdkX86() },
                { Architecture.X64, () => AllWindowsSdkX64() },
                { Architecture.ARM64, () => AllWindowsSdkARM64() }
            };
        }

        public InstalledComponents()
        : this(
            new DefaultVisualStudioFactory(),
            new DefaultWindowsSdkFactory(),
            new DefaultUniversalCrtSdkFactory(),
            new IVisualStudioVersionLocator[2] {
                new VisualStudioVersionsFromRegQuery(),
                new VisualStudioVersionsFromSetupApi()
            },
            new IWindowsSdkVersionLocator[2] {
                new WindowsSdkLegacyVerionsFromRegistry(),
                new WindowsSdk10OrNewerVerionsFromRegistry()
            },
            new UniversalCrtSdkVersionsFromRegistry()
        )
        {

        }

        public IVisualStudioEnumeration AllVisualStudioX64()
        {
            return new VisualStudioEnumeration(AllVisualStudioCore(Architecture.X64));
        }

        public IVisualStudioEnumeration AllVisualStudioX86()
        {
            return new VisualStudioEnumeration(AllVisualStudioCore(Architecture.X86));
        }

        public IVisualStudioEnumeration AllVisualStudioARM64()
        {
            return new VisualStudioEnumeration(AllVisualStudioCore(Architecture.ARM64));
        }

        private IEnumerable<VisualStudio> AllVisualStudioCore(Architecture arch)
        {
            List<VisualStudio> visualStudios = new List<VisualStudio>();

            foreach (IVisualStudioVersionLocator locator in _visualStudioVersionLocators)
            {
                foreach (VisualStudioVersion version in locator.GetVersions())
                {
                    VisualStudio visualStudio = _vsFactory.Create(version, arch);

                    if (visualStudio != null)
                    {
                        visualStudios.Add(visualStudio);
                    }
                }
            }

            visualStudios.Sort(VisualStudioComparers.Descending);

            return visualStudios;
        }

        public IWindowsSdkEnumeration AllWindowsSdkX64()
        {
            return new WindowsSdkEnumeration(AllWindowsSdkCore(Architecture.X64));
        }

        public IWindowsSdkEnumeration AllWindowsSdkX86()
        {
            return new WindowsSdkEnumeration(AllWindowsSdkCore(Architecture.X86));
        }

        public IWindowsSdkEnumeration AllWindowsSdkARM64()
        {
            return new WindowsSdkEnumeration(AllWindowsSdkCore(Architecture.ARM64));
        }

        private IEnumerable<WindowsSdk> AllWindowsSdkCore(Architecture arch)
        {
            // Collect all the UCRT SDKs
            List<WindowsSdk> ucrtSdks = new List<WindowsSdk>();
            foreach (WindowsSdkVersion ucrtSdkVersion in _universalCrtSdkVersionLocator.GetVersions())
            {
                WindowsSdk ucrtSdk = _universalCrtSdkFactory.Create(ucrtSdkVersion, arch);

                if (ucrtSdk != null)
                {
                    ucrtSdks.Add(ucrtSdk);
                }
            }

            // Sort in descending order. After sorting,
            // the first element is going to be
            // the latest UCRT SDK available.
            ucrtSdks.Sort(WindowsSdkComparers.Descending);

            List<WindowsSdk> windowsSdks = new List<WindowsSdk>();

            foreach (IWindowsSdkVersionLocator locator in _windowsSdkVersionLocators)
            {
                foreach (WindowsSdkVersion version in locator.GetVersions())
                {
                    WindowsSdk windowsSdk = null;

                    // If this version already ships with UCRT, then
                    // just create it. Otherwise, if no UCRT SDKs
                    // are available, then just gamble
                    // and let the user try to install using it. Most likely,
                    // it will fail on recent Visual Studio versions,
                    // because the compiler will not be able to find
                    // headers.

                    if (version.HasUniversalCRT || ucrtSdks.Count == 0)
                    {
                        windowsSdk = _winsdkFactory.Create(version, arch);
                    }
                    else
                    {
                        // We should reach here if it is a legacy
                        // Windows SDK (8.1 or older), and there
                        // is a UCRT SDK to use. Since we sorted in
                        // descending order, the first element
                        // is the latest UCRT SDK available.

                        WindowsSdk ucrtSdk = ucrtSdks[0];
                        WindowsSdk legacySdk = _winsdkFactory.Create(version, arch);

                        if (legacySdk != null)
                        {
                            // We just merge the include and libpath
                            // directories of the legacy SDK and UCRT SDK.

                            windowsSdk = new WindowsSdk(
                                version,
                                arch,
                                new IncludeDirectories(UnionStrings(new ReadOnlyStringArray[2] { legacySdk.IncludeDirectories, ucrtSdk.IncludeDirectories })),
                                new LibPathDirectories(UnionStrings(new ReadOnlyStringArray[2] { legacySdk.LibPathDirectories, ucrtSdk.LibPathDirectories }))
                            );
                        }
                    }

                    if (windowsSdk != null)
                    {
                        windowsSdks.Add(windowsSdk);
                    }
                }
            }

            windowsSdks.Sort(WindowsSdkComparers.Descending);

            return windowsSdks;
        }

        private string[] UnionStrings(ReadOnlyStringArray[] arrays)
        {
            int arrayLen, j;
            ReadOnlyStringArray item;
            int len = arrays.Length;

            int totalLen = 0;
            for (int i = 0; i < len; i++)
            {
                totalLen += arrays[i].Length;
            }

            string[] buffer = new string[totalLen];

            int bufferIndex = 0;
            for (int i = 0; i < len; i++)
            {
                item = arrays[i];
                arrayLen = item.Length;

                for (j = 0; j < arrayLen; j++, bufferIndex++)
                {
                    buffer[bufferIndex] = item[j];
                }
            }

            return buffer;
        }

        public IVisualStudioEnumeration AllVisualStudioByArch(Architecture arch)
        {
            Func<IVisualStudioEnumeration> selector = _vsDispatcher[arch];

            if (selector == null)
            {
                throw new ArgumentException("Unknown architecture", "arch");
            }

            return selector();
        }

        public IWindowsSdkEnumeration AllWindowsSdkByArch(Architecture arch)
        {
            Func<IWindowsSdkEnumeration> selector = _winsdkDispatcher[arch];

            if (selector == null)
            {
                throw new ArgumentException("Unknown architecture", "arch");
            }

            return selector();
        }
    }
}
