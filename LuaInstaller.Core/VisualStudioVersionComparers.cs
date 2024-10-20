using System.Collections.Generic;
using System;

namespace LuaInstaller.Core
{
    public static class VisualStudioVersionComparers
    {
        private static readonly StandardComparers<VisualStudioVersion> standardComparers;

        static VisualStudioVersionComparers()
        {
            standardComparers = new StandardComparers<VisualStudioVersion>();
        }

        public static IComparer<VisualStudioVersion> Ascending
        {
            get
            {
                return standardComparers.Ascending;
            }
        }

        public static IComparer<VisualStudioVersion> Descending
        {
            get
            {
                return standardComparers.Descending;
            }
        }
    }
}
