namespace LuaInstaller.Core
{
    /// <summary>
    /// Sets a combination of optimizations that optimizes code for maximum speed. Syntax: &quot;/O2&quot;.
    /// </summary>
    public sealed class VisualStudioO2CompilerOption : VisualStudioOptimizeCodeCompilerOption
    {
        public VisualStudioO2CompilerOption() : base("2")
        {
        }
    }
}
