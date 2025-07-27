using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// Represents an option for a
    /// given compiler.
    /// </summary>
    public abstract class CompilerOption : IComparable<CompilerOption>
    {
        public const int SortOrderCFLAGS = 0;
        public const int SortOrderPREPROCESSORMACRO = 1;
        public const int SortOrderINCLUDEDIR = 2;
        public const int SortOrderSOURCEFILE = 3;

        public abstract int CommandLineSortOrder { get; }

        public int CompareTo(CompilerOption other)
        {
            return ((other == null) ? 1 : (CommandLineSortOrder - other.CommandLineSortOrder));
        }

        public override abstract string ToString();
    }
}
