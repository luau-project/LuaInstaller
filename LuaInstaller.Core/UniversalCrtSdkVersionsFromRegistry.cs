using Microsoft.Win32;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System;

namespace LuaInstaller.Core
{
    public class UniversalCrtSdkVersionsFromRegistry : IUniversalCrtSdkVersionLocator
    {
        private static readonly Regex _rgx;

        static UniversalCrtSdkVersionsFromRegistry()
        {
            _rgx = new Regex(@"^(\d+)\.(\d+)\.(\d+)\.(\d+)$");
        }

#if NET6_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        private WindowsSdkVersion[] GetVersionsCore(string initialReg)
        {
            SortedSet<WindowsSdkVersion> values = new SortedSet<WindowsSdkVersion>(WindowsSdkVersionComparers.Descending);

            using (RegistryKey reg = Registry.LocalMachine.OpenSubKey(initialReg))
            {
                if (reg != null)
                {
                    string installationFolder = (string)(reg.GetValue("KitsRoot10"));

                    if (installationFolder != null)
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
                                string productVersion = subKey;
                                int major = int.Parse(match.Groups[1].Value);

                                if (major == 10)
                                {
                                    int minor = int.Parse(match.Groups[2].Value);
                                    int build = int.Parse(match.Groups[3].Value);
                                    int revision = int.Parse(match.Groups[4].Value);

                                    values.Add(new WindowsSdkVersion(major, minor, installationFolder, productVersion, build, revision));
                                }
                            }
                        }
                    }
                }
            }

            WindowsSdkVersion[] result = new WindowsSdkVersion[values.Count];
            values.CopyTo(result);
            return result;
        }

#if NET6_0_OR_GREATER
        [SupportedOSPlatform("windows")]
#endif
        public WindowsSdkVersion[] GetVersions()
        {
            return GetVersionsCore(Environment.Is64BitOperatingSystem ?
                @"SOFTWARE\WOW6432Node\Microsoft\Windows Kits\Installed Roots" :
                @"SOFTWARE\Microsoft\Microsoft\Windows Kits\Installed Roots"
            );
        }
    }
}
