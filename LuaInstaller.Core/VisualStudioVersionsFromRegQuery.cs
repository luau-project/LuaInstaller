using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace LuaInstaller.Core
{
    public class VisualStudioVersionsFromRegQuery : IVisualStudioVersionLocator
    {
        private static readonly Regex _rgx;

        static VisualStudioVersionsFromRegQuery()
        {
            _rgx = new Regex(@"(\d+)\.(\d+)$");
        }

#if NET6_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        private VisualStudioVersion[] GetVersionsCore(string initialReg)
        {
            SortedSet<VisualStudioVersion> values = new SortedSet<VisualStudioVersion>(VisualStudioVersionComparers.Descending);

            using (RegistryKey reg = Registry.LocalMachine.OpenSubKey(initialReg))
            {
                if (reg != null)
                {
                    string subKey = null;
                    string[] subKeys = reg.GetSubKeyNames();
                    int length = subKeys.Length;

                    for (int i = 0; i < length; i++)
                    {
                        subKey = subKeys[i];

                        Match match = _rgx.Match(subKey);
                        if (match.Success)
                        {
                            using (RegistryKey vsReg = reg.OpenSubKey(subKey + @"\Setup\VS"))
                            {
                                if (vsReg != null)
                                {
                                    string vsDir = (string)(vsReg.GetValue("ProductDir"));

                                    if (vsDir != null && Directory.Exists(vsDir))
                                    {
                                        using (RegistryKey vcReg = reg.OpenSubKey(subKey + @"\Setup\VC"))
                                        {
                                            if (vcReg != null)
                                            {
                                                string vcDir = (string)(vcReg.GetValue("ProductDir"));

                                                if (vcDir != null && Directory.Exists(vcDir))
                                                {
                                                    int major = int.Parse(match.Groups[1].Value);
                                                    int minor = int.Parse(match.Groups[2].Value);
                                                    values.Add(new VisualStudioVersion(major, minor, vsDir, vcDir));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }                
                    }
                }
            }

            VisualStudioVersion[] result = new VisualStudioVersion[values.Count];
            values.CopyTo(result);
            return result;
        }

#if NET6_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public VisualStudioVersion[] GetVersions()
        {
            return GetVersionsCore(Environment.Is64BitOperatingSystem ?
                @"SOFTWARE\WOW6432Node\Microsoft\VisualStudio" :
                @"SOFTWARE\Microsoft\VisualStudio"
            );
        }
    }
}
