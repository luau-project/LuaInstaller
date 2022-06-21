using System;
using System.Collections.Generic;
//using System.Linq;

namespace LuaInstaller.Core
{
    public class InstalledComponents : IInstalledComponents
    {
        private readonly IVisualStudioFactory _vsFactory;
        private readonly IWindowsSdkFactory _winsdkFactory;
        private readonly IVisualStudioVersionLocator[] _visualStudioVersionLocators;
        private readonly IWindowsSdkVersionLocator[] _windowsSdkVersionLocators;
        
        public InstalledComponents(
            IVisualStudioFactory vsFactory,
            IWindowsSdkFactory winsdkFactory,
            IVisualStudioVersionLocator[] visualStudioVersionLocators,
            IWindowsSdkVersionLocator[] windowsSdkVersionLocators
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

            if (visualStudioVersionLocators == null)
            {
                throw new ArgumentNullException("visualStudioVersionLocators");
            }

            if (windowsSdkVersionLocators == null)
            {
                throw new ArgumentNullException("windowsSdkVersionLocators");
            }

			_vsFactory = vsFactory;
			_winsdkFactory = winsdkFactory;

			_visualStudioVersionLocators = new IVisualStudioVersionLocator[visualStudioVersionLocators.Length];
            visualStudioVersionLocators.CopyTo(_visualStudioVersionLocators, 0);

            _windowsSdkVersionLocators = new IWindowsSdkVersionLocator[windowsSdkVersionLocators.Length];
            windowsSdkVersionLocators.CopyTo(_windowsSdkVersionLocators, 0);
		}

		public InstalledComponents()
            : this(
                  new DefaultVisualStudioFactory(),
                  new DefaultWindowsSdkFactory(),
                  new IVisualStudioVersionLocator[2] {
                      new VisualStudioVersionsFromRegQuery(),
                      new VisualStudioVersionsFromSetupApi()
                  },
                  new IWindowsSdkVersionLocator[2] {
                      new WindowsSdkVerionsFromRegQuery(),
                      new WindowsSdkVerionsFromVisualStudioInstall()
                  })
        {

        }

        public IEnumerable<VisualStudio> AllVisualStudioX64()
        {
            return AllVisualStudioCore(Architecture.X64);
        }
        
        public IEnumerable<VisualStudio> AllVisualStudioX86()
        {
            return AllVisualStudioCore(Architecture.X86);
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
            
            visualStudios.Sort();

            return visualStudios;
        }

        public IEnumerable<WindowsSdk> AllWindowsSdkX64()
        {
            return AllWindowsSdkCore(Architecture.X64);
        }

        public IEnumerable<WindowsSdk> AllWindowsSdkX86()
        {
            return AllWindowsSdkCore(Architecture.X86);
        }

        private IEnumerable<WindowsSdk> AllWindowsSdkCore(Architecture arch)
		{
            List<WindowsSdk> windowsSdks = new List<WindowsSdk>();
            
            foreach (IWindowsSdkVersionLocator locator in _windowsSdkVersionLocators)
            {
                foreach (WindowsSdkVersion version in locator.GetVersions())
                {
                    WindowsSdk windowsSdk = _winsdkFactory.Create(version, arch);

                    if (windowsSdk != null)
                    {
                        windowsSdks.Add(windowsSdk);
                    }
                }
            }

            windowsSdks.Sort();

            return windowsSdks;
		}
	}
}
