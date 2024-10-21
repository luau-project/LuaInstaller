using System;

namespace LuaInstaller.Core
{
    public sealed class LuaCompatibility : AbstractLuaCompatibility
    {
        internal LuaCompatibility(string luaCompat) : base(luaCompat)
        {
        }

        public string Value
        {
            get
            {
                return _luaCompat;
            }
        }

        public void AddDefine(ICompiler compiler)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException();
            }

            compiler.AddDefine(Value);
        }
    }
}
