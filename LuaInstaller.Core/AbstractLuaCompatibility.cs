using System.IO;

namespace LuaInstaller.Core
{
    public abstract class AbstractLuaCompatibility
    {
        protected internal readonly string _luaCompat;

        protected AbstractLuaCompatibility(string luaCompat)
        {
            _luaCompat = luaCompat;
        }

        public bool HasCompatibility
        {
            get
            {
                return _luaCompat != null;
            }
        }

        public bool TryAddDefine(ICompiler compiler)
        {
            bool res;
            if (res = (compiler != null && HasCompatibility))
            {
                compiler.AddDefine(_luaCompat);
            }

            return res;
        }
    }
}
