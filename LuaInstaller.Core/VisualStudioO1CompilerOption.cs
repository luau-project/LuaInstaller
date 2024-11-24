namespace LuaInstaller.Core
{
    /// <summary>
    /// Sets a combination of optimizations that generate minimum size code. Syntax: &quot;/O1&quot;.
    /// </summary>
    public sealed class VisualStudioO1CompilerOption : VisualStudioOptimizeCodeCompilerOption
    {
        public VisualStudioO1CompilerOption() : base("1")
        {
        }
    }
}
