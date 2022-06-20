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
        
        public InstalledComponents(IVisualStudioFactory vsFactory, IWindowsSdkFactory winsdkFactory, IVisualStudioVersionLocator[] visualStudioVersionLocators)
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

			_vsFactory = vsFactory;
			_winsdkFactory = winsdkFactory;

            int len = visualStudioVersionLocators.Length;
			_visualStudioVersionLocators = new IVisualStudioVersionLocator[len];
            visualStudioVersionLocators.CopyTo(_visualStudioVersionLocators, 0);
		}

		public InstalledComponents()
            : this(
                  new DefaultVisualStudioFactory(),
                  new DefaultWindowsSdkFactory(),
                  new IVisualStudioVersionLocator[2] {
                      new VisualStudioVersionsFromRegQuery(),
                      new VisualStudioVersionsFromSetupApi()
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
            foreach (IVisualStudioVersionLocator locator in _visualStudioVersionLocators)
            {
                foreach (VisualStudioVersion version in locator.GetVersions())
                {
                    VisualStudio visualStudio = _vsFactory.Create(version, arch);

                    if (visualStudio != null)
                    {
                        yield return visualStudio;
                    }
                }
            }
            //return from vsVer in VisualStudioVersionsFromRegQuery.GetVersions()
            //       let visualStudio = _vsFactory.Create(vsVer, arch)
            //       where visualStudio != null
            //       select visualStudio;
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
            foreach (WindowsSdkVersion windowsSdkVersion in WindowsSdkRegQuery.GetVersions())
            {
                WindowsSdk windowsSdk = _winsdkFactory.Create(windowsSdkVersion, arch);

                if (windowsSdk != null)
                {
                    yield return windowsSdk;
                }
            }
			//return from winsdkVer in WindowsSdkRegQuery.GetVersions()
			//       let winsdk = _winsdkFactory.Create(winsdkVer, arch)
			//       where winsdk != null
			//       select winsdk;
		}
	}
}
