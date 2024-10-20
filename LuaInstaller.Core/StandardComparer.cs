using System.Collections.Generic;
using System;

namespace LuaInstaller.Core
{
    public class StandardComparer<T> : IComparer<T>
        where T : IComparable<T>
    {
        private readonly Func<T, T, int> _comparer;

        public StandardComparer(Func<T, T, int> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException();
            }

            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }
}
