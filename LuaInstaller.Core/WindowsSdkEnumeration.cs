using System;
using System.Collections;
using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public class WindowsSdkEnumeration : IWindowsSdkEnumeration
    {
        private readonly SortedSet<WindowsSdk> _windowsSdks = new SortedSet<WindowsSdk>();

        public WindowsSdkEnumeration(IEnumerable<WindowsSdk> windowsSdks)
        {
            if (windowsSdks == null)
            {
                throw new ArgumentNullException();
            }

            foreach (WindowsSdk windowsSdk in windowsSdks)
            {
                if (windowsSdk == null)
                {
                    throw new ArgumentException("Contains null WindowsSdk element");
                }
            }

            _windowsSdks = new SortedSet<WindowsSdk>(windowsSdks, WindowsSdkComparers.Descending);
        }

        public IEnumerator<WindowsSdk> GetEnumerator()
        {
            return _windowsSdks.GetEnumerator();
        }

        public bool TryGetLatest(out WindowsSdk sdk)
        {
            bool hasAny = _windowsSdks.Count > 0;
            sdk = hasAny ? _windowsSdks.Min : null;
            return hasAny;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _windowsSdks.GetEnumerator();
        }
    }
}
