namespace LuaInstaller.Core
{
    /// <summary>
    /// Prevents the automatic call to the linker. Syntax: &quot;/c&quot;
    /// </summary>
    public sealed class VisualStudioCompileOnlyCompilerOption : CompilerOption
    {
        public override string ToString()
        {
            return "/c";
        }
    }
}
