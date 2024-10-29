using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public interface IVisualStudioEnumeration : IEnumerable<VisualStudio>
    {
        bool TryGetLatest(out VisualStudio visualStudio);
    }
}
