using System.Collections.Generic;

namespace LuaInstaller.Core
{
    public static class VisualStudioComparers
    {
        private static readonly StandardComparers<VisualStudio> standardComparers;

        static VisualStudioComparers()
        {
            standardComparers = new StandardComparers<VisualStudio>();
        }

        public static IComparer<VisualStudio> Ascending
        {
            get
            {
                return standardComparers.Ascending;
            }
        }

        public static IComparer<VisualStudio> Descending
        {
            get
            {
                return standardComparers.Descending;
            }
        }
    }
}
