using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// Indicates whether a multithreaded module is a DLL and specifies retail or debug versions of the run-time library.
    /// </summary>
    public class VisualStudioRuntimeLibraryCompilerOption : CompilerOption
    {
        private readonly string _prefix;
        private readonly bool _debug;

        protected VisualStudioRuntimeLibraryCompilerOption(string prefix, bool debug)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            _prefix = prefix;
            _debug = debug;
        }

        public override string ToString()
        {
            return string.Format("/{0}{1}", _prefix, _debug ? "d" : string.Empty);
        }
    }
}
