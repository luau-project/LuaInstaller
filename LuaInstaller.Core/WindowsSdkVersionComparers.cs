using System.Collections.Generic;
using System;

namespace LuaInstaller.Core
{
    public static class WindowsSdkVersionComparers
    {
        private static readonly StandardComparers<WindowsSdkVersion> standardComparers;

        static WindowsSdkVersionComparers()
        {
            standardComparers = new StandardComparers<WindowsSdkVersion>();
        }

        public static IComparer<WindowsSdkVersion> Ascending
        {
            get
            {
                return standardComparers.Ascending;
            }
        }

        public static IComparer<WindowsSdkVersion> Descending
        {
            get
            {
                return standardComparers.Descending;
            }
        }
    }
}
