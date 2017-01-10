using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public interface IInstalledComponents
    {
        IEnumerable<VisualStudio> AllVisualStudioX86();
        IEnumerable<WindowsSdk> AllWindowsSdkX86();

        IEnumerable<VisualStudio> AllVisualStudioX64();
        IEnumerable<WindowsSdk> AllWindowsSdkX64();
    }
}
