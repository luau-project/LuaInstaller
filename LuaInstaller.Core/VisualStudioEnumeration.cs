using System;
using System.Collections;
using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public class VisualStudioEnumeration : IVisualStudioEnumeration
    {
        private readonly SortedSet<VisualStudio> _visualStudios;

        public VisualStudioEnumeration(IEnumerable<VisualStudio> visualStudios)
        {
            if (visualStudios == null)
            {
                throw new ArgumentNullException();
            }

            foreach (VisualStudio vs in visualStudios)
            {
                if (vs == null)
                {
                    throw new ArgumentException("Contains null VisualStudio element");
                }
            }

            _visualStudios = new SortedSet<VisualStudio>(visualStudios, VisualStudioComparers.Descending);
        }

        public IEnumerator<VisualStudio> GetEnumerator()
        {
            return _visualStudios.GetEnumerator();
        }

        public bool TryGetLatest(out VisualStudio visualStudio)
        {
            bool hasAny = _visualStudios.Count > 0;
            visualStudio = hasAny ? _visualStudios.Min : null;
            return hasAny;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _visualStudios.GetEnumerator();
        }
    }
}
