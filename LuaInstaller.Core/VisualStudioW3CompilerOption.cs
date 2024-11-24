namespace LuaInstaller.Core
{
    /// <summary>
    /// Displays level 1, level 2, and level 3 (production quality) warnings.
    /// </summary>
    public sealed class VisualStudioW3CompilerOption : VisualStudioWarningLevelCompilerOption
    {
        public VisualStudioW3CompilerOption() : base("3")
        {
        }
    }
}
