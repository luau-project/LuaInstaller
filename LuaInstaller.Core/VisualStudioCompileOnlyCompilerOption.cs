namespace LuaInstaller.Core
{
    /// <summary>
    /// Prevents the automatic call to the linker. Syntax: &quot;/c&quot;
    /// </summary>
    public sealed class VisualStudioCompileOnlyCompilerOption : CompilerOption
    {
        public override int CommandLineSortOrder { get { return CompilerOption.SortOrderCFLAGS; } }
        public override string ToString()
        {
            return "/c";
        }
    }
}
