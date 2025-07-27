using System;

namespace LuaInstaller.Core
{
    public sealed class VisualStudioIncludeDirectoryCompilerOption : CompilerOption
    {
        private readonly string _includeDirectory;

        public VisualStudioIncludeDirectoryCompilerOption(string includeDirectory)
        {
            if (includeDirectory == null)
            {
                throw new ArgumentNullException();
            }

            _includeDirectory = includeDirectory;
        }
        public override int CommandLineSortOrder { get { return CompilerOption.SortOrderINCLUDEDIR; } }
        public override string ToString()
        {
            return "/I" + _includeDirectory;
        }
    }
}
