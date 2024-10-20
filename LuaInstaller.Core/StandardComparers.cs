using System;
using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public class StandardComparers<T>
        where T: IComparable<T>
    {

        private readonly IComparer<T> ascending;
        private readonly IComparer<T> descending;

        public StandardComparers()
        {
            ascending = new StandardComparer<T>((a, b) => a.CompareTo(b));
            descending = new StandardComparer<T>((a, b) => b.CompareTo(a));
        }

        public IComparer<T> Ascending
        {
            get
            {
                return ascending;
            }
        }

        public IComparer<T> Descending
        {
            get
            {
                return descending;
            }
        }
    }
}
