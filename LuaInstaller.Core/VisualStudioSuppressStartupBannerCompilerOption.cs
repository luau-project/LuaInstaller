namespace LuaInstaller.Core
{
    /// <summary>
    /// Suppresses the display of the
    /// copyright banner when the compiler
    /// starts up and display of informational
    /// messages during compiling. Syntax: &quot;/nologo&quot;.
    /// </summary>
    public sealed class VisualStudioSuppressStartupBannerCompilerOption : CompilerOption
    {
        public override string ToString()
        {
            return "/nologo";
        }
    }
}
