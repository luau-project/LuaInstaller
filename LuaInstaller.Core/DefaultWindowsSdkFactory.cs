﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LuaInstaller.Core
{
    public class DefaultWindowsSdkFactory : IWindowsSdkFactory
    {
        private static readonly Regex _productVersionRgx;

        static DefaultWindowsSdkFactory()
        {
            _productVersionRgx = new Regex(@"^(\d+)\.(\d+)\.(\d+)$");
        }
        
        public WindowsSdk Create(WindowsSdkVersion version, Architecture arch)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            WindowsSdk windowsSdk = null;

            if (arch == Architecture.X86 || arch == Architecture.X64)
            {
                string productDir = null;

                if (version.Major >= 10)
                {
                    Match match = _productVersionRgx.Match(version.ProductVersion);
                    if (match.Success)
                    {
                        productDir = version.ProductVersion + ".0";
                    }
                    else
                    {
                        productDir = version.ProductVersion;
                    }
                }
                else if (version.Major == 8)
                {
                    if (version.Minor == 1)
                    {
                        productDir = "winv6.3";
                    }
                    else if (version.Minor == 0)
                    {
                        productDir = "win8";
                    }
                }

                if (productDir != null)
                {
                    string includeProductDir = version.Major >= 10 ?  Path.Combine(version.InstallationDir, "Include", productDir) : Path.Combine(version.InstallationDir, "Include");
                    string libProductDir = Path.Combine(version.InstallationDir, "Lib", productDir);

                    if (Directory.Exists(includeProductDir) && Directory.Exists(libProductDir))
                    {
                        IncludeDirectories includeDirectories = new IncludeDirectories(
                            Directory.EnumerateDirectories(includeProductDir).ToArray()
                        );

                        string archStr = arch.ToString();

                        LibPathDirectories libPathDirectories = new LibPathDirectories(
                            (
                                from dir in Directory.EnumerateDirectories(libProductDir)
                                let path = Path.Combine(dir, archStr)
                                where Directory.Exists(path)
                                select path
                            ).ToArray()
                        );
                        
                        windowsSdk = new WindowsSdk(version, arch, includeDirectories, libPathDirectories);
                    }
                }
            }

            return windowsSdk;
        }
    }
}
