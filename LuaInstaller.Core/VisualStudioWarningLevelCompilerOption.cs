using System;

namespace LuaInstaller.Core
{
    /// <summary>
    /// Specifies how the compiler generates warnings for a given compilation.
    /// </summary>
    public class VisualStudioWarningLevelCompilerOption : CompilerOption
    {
        private readonly string _level;

        protected VisualStudioWarningLevelCompilerOption(string level)
        {
            if (level == null)
            {
                throw new ArgumentNullException();
            }

            _level = level;
        }

        public override string ToString()
        {
            return "/W" + _level;
        }
    }
}
