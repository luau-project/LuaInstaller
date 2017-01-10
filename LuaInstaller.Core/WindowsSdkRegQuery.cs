using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LuaInstaller.Core
{
    public static class WindowsSdkRegQuery
    {
        private static readonly Regex _rgx;

        static WindowsSdkRegQuery()
        {
            _rgx = new Regex(@"[vV](\d+)\.(\d+)$");
        }

        private static WindowsSdkVersion[] GetVersionsCore(string initialReg)
        {
            SortedSet<WindowsSdkVersion> values = new SortedSet<WindowsSdkVersion>();

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

                                    if (installationFolder != null && Directory.Exists(installationFolder) && productVersion != null)
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

        public static WindowsSdkVersion[] GetVersions()
        {
            return GetVersionsCore(Environment.Is64BitOperatingSystem ?
                @"SOFTWARE\WOW6432Node\Microsoft\Microsoft SDKs\Windows" :
                @"SOFTWARE\Microsoft\Microsoft SDKs\Windows"
            );
        }
    }
}
