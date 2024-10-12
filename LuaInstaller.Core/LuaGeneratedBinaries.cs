using System;

namespace LuaInstaller.Core
{
    public class LuaGeneratedBinaries
    {
        private const string LUA_DLL_NAME_FORMAT = "lua{0}.dll";
        private const string LUA_LIB_NAME_FORMAT = "lua{0}.lib";
        private const string LUA_INTERPRETER_NAME = "lua.exe";
        private const string LUA_COMPILER_NAME = "luac.exe";

        private readonly string _dllName;
        private readonly string _importLibName;
        private readonly string _interpreterName;
        private readonly string _compilerName;

        public LuaGeneratedBinaries(LuaVersion version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            string shortVer = version.ShortVersionWithoutDot;

            _dllName = string.Format(LUA_DLL_NAME_FORMAT, shortVer);
            _importLibName = string.Format(LUA_LIB_NAME_FORMAT, shortVer);
            _interpreterName = LUA_INTERPRETER_NAME;
            _compilerName = LUA_COMPILER_NAME;
        }

        public string DllName
        {
            get 
            { 
                return _dllName; 
            }
        }

        public string ImportLibName
        {
            get 
            { 
                return _importLibName; 
            }
        }

        public string InterpreterName
        {
            get
            { 
                return _interpreterName; 
            }
        }

        public string CompilerName
        {
            get
            {
                return _compilerName;
            }
        }
    }
}
