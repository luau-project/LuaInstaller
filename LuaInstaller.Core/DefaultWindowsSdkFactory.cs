using System;
using System.IO;
using System.Linq;

namespace LuaInstaller.Core
{
    public class DefaultWindowsSdkFactory : IWindowsSdkFactory
    {
        public WindowsSdk Create(WindowsSdkVersion version, Architecture arch)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            WindowsSdk sdk = null;

            if (arch == Architecture.X86 || arch == Architecture.X64)
            {
                string includeDir = Path.Combine(version.InstallationDir, "include");
                string libDir = Path.Combine(version.InstallationDir, "lib");

                string includeProdVersionDir = Path.Combine(includeDir, version.ProductVersion);
                string libProdVersionDir = Path.Combine(libDir, version.ProductVersion);

                DirectoryInfo includeInfo = null;
                DirectoryInfo libInfo = null;

                if (Directory.Exists(includeProdVersionDir) &&
                    Directory.Exists(libProdVersionDir))
                {
                    includeInfo = new DirectoryInfo(includeProdVersionDir);
                    libInfo = new DirectoryInfo(libProdVersionDir);
                }
                else
                {
                    includeInfo = (
                        from versionDir in Directory.EnumerateDirectories(includeDir)
                        where Path.GetFileName(versionDir).StartsWith(version.ProductVersion)
                        select new DirectoryInfo(versionDir)
                    ).FirstOrDefault();

                    libInfo = (
                        from versionDir in Directory.EnumerateDirectories(libDir)
                        where Path.GetFileName(versionDir).StartsWith(version.ProductVersion)
                        select new DirectoryInfo(versionDir)
                    ).FirstOrDefault();

                    if (includeInfo == null || libInfo == null)
                    {
                        includeInfo = new DirectoryInfo(includeDir);
                        libInfo = new DirectoryInfo(libDir).EnumerateDirectories().LastOrDefault();
                    }
                }

                if (includeInfo != null && libInfo != null)
                {
                    IncludeDirectories includeDirectories = new IncludeDirectories(
                        (
                            from dirInfo in includeInfo.EnumerateDirectories()
                            select dirInfo.FullName
                        ).ToArray()
                    );

                    LibPathDirectories libDirectories = new LibPathDirectories(
                        (
                            from dirInfo in libInfo.EnumerateDirectories()
                            let path = Path.Combine(dirInfo.FullName, arch.ToString())
                            where Directory.Exists(path)
                            select path
                        ).ToArray()
                    );

                    if (includeDirectories.Length > 0 && libDirectories.Length > 0)
                    {
                        sdk = new WindowsSdk(version, arch, includeDirectories, libDirectories);
                    }
                }
            }

            return sdk;
        }
    }
}
