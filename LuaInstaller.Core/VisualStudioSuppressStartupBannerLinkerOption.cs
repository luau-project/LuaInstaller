namespace LuaInstaller.Core
{
    /// <summary>
    /// Suppresses the display of the
    /// copyright banner when the linker
    /// starts up and display of informational
    /// messages during linking. Syntax: &quot;/nologo&quot;.
    /// </summary>
    public sealed class VisualStudioSuppressStartupBannerLinkerOption : LinkerOption
    {
        public override int CommandLineSortOrder { get { return LinkerOption.SortOrderLDFLAGS; } }
        public override string ToString()
        {
            return "/nologo";
        }
    }
}
