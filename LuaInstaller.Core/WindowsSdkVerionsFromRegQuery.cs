using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace LuaInstaller.Core
{
    public class WindowsSdkVerionsFromRegQuery : IWindowsSdkVersionLocator
    {
        private static readonly Regex _rgx;

        static WindowsSdkVerionsFromRegQuery()
        {
            _rgx = new Regex(@"^[vV](\d+)\.(\d+)$");
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
                    string subKey = null;
                    string[] subKeys = reg.GetSubKeyNames();
                    int length = subKeys.Length;

                    for (int i = 0; i < length; i++)
                    {
                        subKey = subKeys[i];

                        Match match = _rgx.Match(subKey);
                        if (match.Success)
                        {
                            using (RegistryKey sdkReg = reg.OpenSubKey(subKey))
                            {
                                if (sdkReg != null)
                                {
                                    string installationFolder = (string)(sdkReg.GetValue("InstallationFolder"));
                                    string productVersion = (string)(sdkReg.GetValue("ProductVersion"));

                                    if (installationFolder != null && productVersion != null)
                                    {
                                        int major = int.Parse(match.Groups[1].Value);
                                        int minor = int.Parse(match.Groups[2].Value);

                                        values.Add(new WindowsSdkVersion(major, minor, installationFolder, productVersion));
                                    }
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
                @"SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows" :
                @"SOFTWARE\Microsoft\Microsoft SDKs\Windows"
            );
        }
    }
}
