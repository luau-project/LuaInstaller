using System;
using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public static class LuaVersionComparers
    {
        private static readonly StandardComparers<LuaVersion> standardComparers;

        static LuaVersionComparers()
        {
            standardComparers = new StandardComparers<LuaVersion>();
        }

        public static IComparer<LuaVersion> Ascending
        {
            get
            {
                return standardComparers.Ascending;
            }
        }

        public static IComparer<LuaVersion> Descending
        {
            get
            {
                return standardComparers.Descending;
            }
        }
    }
}
