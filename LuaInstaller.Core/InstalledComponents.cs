using System;
using System.Collections.Generic;
using System.Linq;

namespace LuaInstaller.Core
{
    public class InstalledComponents : IInstalledComponents
    {
        private readonly IVisualStudioFactory _vsFactory;
        private readonly IWindowsSdkFactory _winsdkFactory;
        
        public InstalledComponents(IVisualStudioFactory vsFactory, IWindowsSdkFactory winsdkFactory)
        {
            if (vsFactory == null)
            {
                throw new ArgumentNullException("vsFactory");
            }

            if (winsdkFactory == null)
            {
                throw new ArgumentNullException("winsdkFactory");
            }

            _vsFactory = vsFactory;
            _winsdkFactory = winsdkFactory;
        }

        public InstalledComponents()
            : this(new DefaultVisualStudioFactory(), new DefaultWindowsSdkFactory())
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
            return from vsVer in VisualStudioRegQuery.GetVersions()
                   let vs = _vsFactory.Create(vsVer, arch)
                   where vs != null
                   select vs;
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
            return from winsdkVer in WindowsSdkRegQuery.GetVersions()
                   let winsdk = _winsdkFactory.Create(winsdkVer, arch)
                   where winsdk != null
                   select winsdk;
        }
    }
}
