namespace LuaInstaller.Core
{
    /// <summary>
    /// Defines a preprocessing symbol in the source code.
    /// Syntax: &quot;/Dname&quot; or &quot;/Dname=value&quot; depending whether value was provided or not.
    /// </summary>
    public sealed class VisualStudioPreprocessorDefinitionCompilerOption : CompilerOption
    {
        private readonly string _name;
        private readonly string _value;

        /// <summary>
        /// Defines a preprocessing symbol for <paramref name="name"/> in the source code.
        /// </summary>
        /// <param name="name">The symbol name to be defined</param>
        public VisualStudioPreprocessorDefinitionCompilerOption(string name) : this(name, null)
        {

        }

        /// <summary>
        /// Defines a preprocessing symbol for <paramref name="name"/> in the source code containing <paramref name="value"/> as value.
        /// </summary>
        /// <param name="name">The symbol name to be defined</param>
        /// <param name="value">The symbol value to be defined</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        public VisualStudioPreprocessorDefinitionCompilerOption(string name, string value)
        {
            if (name == null)
            {
                throw new System.ArgumentNullException("name");
            }

            _name = name;
            _value = value;
        }
        public override int CommandLineSortOrder { get { return CompilerOption.SortOrderPREPROCESSORMACRO; } }
        public override string ToString()
        {
            return _value == null ?
                string.Format("/D{0}", _name) :
                string.Format("/D{0}={1}", _name, _value);
        }
    }
}
