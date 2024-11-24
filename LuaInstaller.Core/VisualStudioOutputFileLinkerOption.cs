using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// This option overrides the default name and location of the program that the linker creates.
    /// Syntax: &quot;/OUT:filename&quot;, such that filename is supplied by the user.
    /// </summary>
    public sealed class VisualStudioOutputFileLinkerOption : LinkerOption
    {
        private readonly string _outputFile;

        /// <summary>
        /// Specifies an output file for the linker.
        /// </summary>
        /// <param name="outputFile">The filename of the output file.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="outputFile"/> is null.</exception>
        public VisualStudioOutputFileLinkerOption(string outputFile)
        {
            if (outputFile == null)
            {
                throw new ArgumentNullException();
            }

            _outputFile = outputFile;
        }

        public override string ToString()
        {
            return "/OUT:" + _outputFile;
        }
    }
}
