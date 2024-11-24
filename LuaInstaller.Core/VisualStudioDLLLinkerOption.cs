using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaInstaller.Core
{
    /// <summary>
    /// This option builds a DLL as the main output file.
    /// Syntax: &quot;/DLL&quot;.
    /// </summary>
    public sealed class VisualStudioDLLLinkerOption : LinkerOption
    {
        public override string ToString()
        {
            return "/DLL";
        }
    }
}
