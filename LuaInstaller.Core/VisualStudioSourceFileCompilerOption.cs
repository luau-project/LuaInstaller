using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// Represents a source file input for the compiler.
    /// </summary>
    public class VisualStudioSourceFileCompilerOption : CompilerOption
    {
        private readonly string _srcFile;

        /// <summary>
        /// Creates a source file input for the compiler.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceFile"/> is null.</exception>
        public VisualStudioSourceFileCompilerOption(string sourceFile)
        {
            if (sourceFile == null)
            {
                throw new ArgumentNullException();
            }

            _srcFile = sourceFile;
        }

        public override string ToString()
        {
            return _srcFile;
        }
    }
}
