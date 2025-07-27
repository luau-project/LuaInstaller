using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// Represents an option for a
    /// given linker.
    /// </summary>
    public abstract class LinkerOption : IComparable<LinkerOption>
    {
        public const int SortOrderLDFLAGS = 0;
        public const int SortOrderLIBDIR = 1;
        public const int SortOrderOUTPUTFILE = 2;
        public const int SortOrderOBJECTFILE = 3;
        public const int SortOrderLIBFILE = 4;
        public const int SortOrderDEPENDENTLIB = 5;

        public abstract int CommandLineSortOrder { get; }

        public int CompareTo(LinkerOption other)
        {
            return ((other == null) ? 1 : (CommandLineSortOrder - other.CommandLineSortOrder));
        }

        public override abstract string ToString();
    }
}
