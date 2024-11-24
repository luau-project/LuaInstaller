namespace LuaInstaller.Core
{
    /// <summary>
    /// Causes the application to use the multithread-specific
    /// and DLL-specific version of the run-time library.
    /// Syntax: &quot;/MD&quot; for retail or &quot;/MDd&quot; for debug.
    /// </summary>
    public sealed class VisualStudioMultithreadedDLLRuntimeLibraryCompilerOption : VisualStudioRuntimeLibraryCompilerOption
    {
        /// <summary>
        /// Creates a runtime library
        /// compiler option instance (&quot;/MD&quot; for retail or &quot;/MDd&quot; for debug),
        /// depending whether <paramref name="debug"/> is selected or not.
        /// </summary>
        /// <param name="debug">Causes </param>
        public VisualStudioMultithreadedDLLRuntimeLibraryCompilerOption(bool debug) : base("MD", debug)
        {
        }
    }
}
