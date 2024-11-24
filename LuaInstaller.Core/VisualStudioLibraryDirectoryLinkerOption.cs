using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// Specifies a path that the linker will search.
    /// Syntax: &quot;/LIBPATH:dir&quot;, such that dir is a
    /// directory provided by the user.
    /// </summary>
    public sealed class VisualStudioLibraryDirectoryLinkerOption : LinkerOption
    {
        private readonly string _libraryDirectory;

        /// <summary>
        /// Specifies a path to the linker with a directory to search for libraries.
        /// </summary>
        /// <param name="libraryDirectory">The directory to search for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="libraryDirectory"/> is null.</exception>
        public VisualStudioLibraryDirectoryLinkerOption(string libraryDirectory)
        {
            if (libraryDirectory == null)
            {
                throw new ArgumentNullException();
            }

            _libraryDirectory = libraryDirectory;
        }

        public override string ToString()
        {
            return "/LIBPATH:" + _libraryDirectory;
        }
    }
}
