using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// An input file for the linker.
    /// </summary>
    public sealed class VisualStudioInputFileLinkerOption : LinkerOption
    {
        private readonly string _inputFile;

        /// <summary>
        /// Creates an input file for the linker.
        /// </summary>
        /// <param name="inputFile">The input file to feed the linker</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputFile"/> is null.</exception>
        public VisualStudioInputFileLinkerOption(string inputFile)
        {
            if (inputFile == null)
            {
                throw new ArgumentNullException();
            }

            _inputFile = inputFile;
        }

        public override string ToString()
        {
            return _inputFile;
        }
    }
}
