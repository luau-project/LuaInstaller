using System.Collections.Generic;
using System;

namespace LuaInstaller.Core
{
    public static class WindowsSdkComparers
    {
        private static readonly StandardComparers<WindowsSdk> standardComparers;

        static WindowsSdkComparers()
        {
            standardComparers = new StandardComparers<WindowsSdk>();
        }

        public static IComparer<WindowsSdk> Ascending
        {
            get
            {
                return standardComparers.Ascending;
            }
        }

        public static IComparer<WindowsSdk> Descending
        {
            get
            {
                return standardComparers.Descending;
            }
        }
    }
}
