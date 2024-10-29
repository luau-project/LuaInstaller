using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LuaInstaller.Core
{
    public class DefaultUniversalCrtSdkFactory : IUniversalCrtSdkFactory
    {
        private static readonly Regex _productVersionRgx;

        static DefaultUniversalCrtSdkFactory()
        {
            _productVersionRgx = new Regex(@"^(\d+)\.(\d+)\.(\d+)$");
        }

        public WindowsSdk Create(WindowsSdkVersion version, Architecture arch)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            if (!version.HasUniversalCRT)
            {
                throw new ArgumentException("Universal CRT was expected for the windows sdk version", "version");
            }

            WindowsSdk windowsSdk = null;

            if (arch == Architecture.X86 || arch == Architecture.X64)
            {
                string productDir = null;

                Match match = _productVersionRgx.Match(version.ProductVersion);
                if (match.Success)
                {
                    productDir = version.ProductVersion + ".0";
                }
                else
                {
                    productDir = version.ProductVersion;
                }

                string includeProductDir = Path.Combine(version.InstallationDir, "Include", productDir);
                string libProductDir = Path.Combine(version.InstallationDir, "Lib", productDir);

                if (Directory.Exists(includeProductDir) && Directory.Exists(libProductDir))
                {
                    IncludeDirectories includeDirectories = new IncludeDirectories(
                        new string[1] { Path.Combine(includeProductDir, "ucrt") }
                    );

                    string archStr = arch.ToString();

                    LibPathDirectories libPathDirectories = new LibPathDirectories(
                        new string[1] { Path.Combine(libProductDir, "ucrt", archStr) }
                    );

                    windowsSdk = new WindowsSdk(version, arch, includeDirectories, libPathDirectories);
                }
            }

            return windowsSdk;
        }
    }
}
