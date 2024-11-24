namespace LuaInstaller.Core
{
    /// <summary>
    /// Causes the application to use the multithread, static version of the run-time library.
    /// Syntax: &quot;/MT&quot; for retail or &quot;/MTd&quot; for debug.
    /// </summary>
    public sealed class VisualStudioMultithreadedStaticRuntimeLibraryCompilerOption : VisualStudioRuntimeLibraryCompilerOption
    {
        /// <summary>
        /// Creates a runtime library
        /// compiler option instance (&quot;/MT&quot; for retail or &quot;/MTd&quot; for debug),
        /// depending whether <paramref name="debug"/> is selected or not.
        /// </summary>
        /// <param name="debug">Causes </param>
        public VisualStudioMultithreadedStaticRuntimeLibraryCompilerOption(bool debug) : base("MT", debug)
        {
        }
    }
}
