using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public interface IWindowsSdkEnumeration : IEnumerable<WindowsSdk>
    {
        bool TryGetLatest(out WindowsSdk sdk);
    }
}
