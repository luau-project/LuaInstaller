using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// The optmize code options control various optimizations that help you create code for maximum speed or minimum size.
    /// </summary>
    public class VisualStudioOptimizeCodeCompilerOption : CompilerOption
    {
        private readonly string _optimizationSuffix;

        protected VisualStudioOptimizeCodeCompilerOption(string optimizationSuffix)
        {
            if (optimizationSuffix == null)
            {
                throw new ArgumentNullException();
            }

            _optimizationSuffix = optimizationSuffix;
        }
        public override int CommandLineSortOrder { get { return CompilerOption.SortOrderCFLAGS; } }
        public override string ToString()
        {
            return "/O" + _optimizationSuffix;
        }
    }
}
