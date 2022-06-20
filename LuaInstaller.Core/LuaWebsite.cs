using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace LuaInstaller.Core
{
    public static class LuaWebsite
    {
        private const string LUA_DOWNLOAD_URL = "http://www.lua.org/ftp";

        public static LuaVersion[] QueryVersions()
        {
            SortedSet<LuaVersion> result = new SortedSet<LuaVersion>();

            HttpWebRequest request = (HttpWebRequest)(WebRequest.Create(LUA_DOWNLOAD_URL));
            request.Method = "GET";

            using (WebResponse webResponse = request.GetResponse())
            {
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                string response = responseReader.ReadToEnd();

                MatchCollection matches = Regex.Matches(response, @"lua-(\d+)\.(\d+)(\.(\d+))?\.tar\.gz");

                foreach (Match m in matches)
                {
                    int major = int.Parse(m.Groups[1].Value);
                    int minor = int.Parse(m.Groups[2].Value);

                    if (major > 5 || (major == 5 && minor >= 1))
                    {
                        int? build = null;

                        if (m.Groups[4].Success)
                        {
                            build = int.Parse(m.Groups[4].Value);
                        }

                        string downloadUrl = LUA_DOWNLOAD_URL + "//" + m.Value;

                        LuaVersion version = new LuaVersion(major, minor, build, downloadUrl);

                        if (!result.Contains(version))
                        {
                            result.Add(version);
                        }
                    }
                }
            }

            LuaVersion[] array = new LuaVersion[result.Count];
            result.CopyTo(array);
            return array;
        }

        public static LuaVersion GetLatestVersion()
        {
            return QueryVersions()[0];
        }
        public static LuaVersion FindVersion(string version)
        {
            LuaVersion[] versions = QueryVersions();
            return Array.Find(versions, v => v.Version == version);
        }

        public static void DownloadAndExtract(string destDir, string workingDir, LuaVersion version)
        {
            using (WebClient webClient = new WebClient())
            {
                string fileName = Path.Combine(workingDir, string.Format("{0}.tar.gz", version.ExtractedDirectoryName));

                webClient.DownloadFile(
                    new Uri(version.DownloadUrl),
                    fileName
                );

                using (FileStream fs = File.OpenRead(fileName))
                {
                    using (GZipInputStream gzs = new GZipInputStream(fs))
                    {
                        using (TarArchive tar = TarArchive.CreateInputTarArchive(gzs, null))
                        {
                            tar.ExtractContents(destDir);
                        }
                    }
                }
            }
        }
    }
}
