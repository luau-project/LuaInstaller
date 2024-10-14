using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace LuaInstaller.Core
{
    public static class LuaWebsite
    {
        private const double MIN_BASE = 1.7;
        private const double MAX_BASE = 2.3;
        private const int MAX_RETRIES = 3;

        private const string LUA_DOWNLOAD_URL = "https://www.lua.org/ftp";
        private static readonly HttpClient _client;
        private static readonly LuaVersion[] _empty;

        static LuaWebsite()
        {
            _client = new HttpClient();
            _empty = new LuaVersion[0];
        }

        private static int GenerateRandomInt32()
        {
            byte[] buffer = new byte[sizeof(int)];
            
            using (RandomNumberGenerator rgn = RandomNumberGenerator.Create())
            {
                rgn.GetBytes(buffer);
            }

            return BitConverter.ToInt32(buffer, 0);
        }

        private static double GetNextDoubleBetween(double fromInclusive, double toExclusive, Random random)
        {
            return fromInclusive + random.NextDouble() * (toExclusive - fromInclusive);
        }

        private static bool RetryStrategy(Action action, int maxRetries)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            int retries = 1;

            Random random = null;

            bool success = false;

            do
            {
                try
                {
                    action();

                    success = true;
                }
                catch
                {
                    if (random == null)
                    {
                        int seed = GenerateRandomInt32();
                        random = new Random(seed);
                    }

                    // a random number B between 1.7 and 2.3
                    double B = GetNextDoubleBetween(MIN_BASE, MAX_BASE, random);
                    
                    // B raised to the number of retries: B^(retries).
                    // Since retries starts from 1, B^1 = B is the shortest possible
                    // waiting time.
                    double waitingTimeInSeconds = Math.Pow(B, retries);

                    int waitingTimeInMilliseconds = (int)(waitingTimeInSeconds * 1000);

                    Thread.Sleep(waitingTimeInMilliseconds);
                }
            }
            while (++retries <= maxRetries && !success);

            return success;
        }

        private static LuaVersion[] GetVersionsFromHtmlString(string html)
        {
            SortedSet<LuaVersion> result = new SortedSet<LuaVersion>();

            MatchCollection matches = Regex.Matches(html, @"lua-(\d+)\.(\d+)(\.(\d+))?\.tar\.gz");

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

                    string downloadUrl = string.Format("{0}/{1}", LUA_DOWNLOAD_URL, m.Value);

                    LuaVersion version = new LuaVersion(major, minor, build, downloadUrl);

                    if (!result.Contains(version))
                    {
                        result.Add(version);
                    }
                }
            }

            LuaVersion[] array = new LuaVersion[result.Count];
            result.CopyTo(array);
            return array;
        }

        public static LuaVersion[] QueryVersions()
        {
            string html = null;

            bool succeed = RetryStrategy(
                () =>
                {
                    html = _client.GetStringAsync(LUA_DOWNLOAD_URL).Result;
                },
                MAX_RETRIES
            );

            return (!succeed || html == null) ? _empty : GetVersionsFromHtmlString(html);
        }

        public static bool TryGetLatestVersion(out LuaVersion version)
        {
            LuaVersion[] versions = QueryVersions();

            bool result = versions.Length > 0;
            version = result ? versions[0] : null;

            return result;
        }
        
        [ObsoleteAttribute("This method is deprecated. Use TryGetLatestVersion instead.")]
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
            string fileName = Path.Combine(workingDir, string.Format("{0}.tar.gz", version.ExtractedDirectoryName));

            bool success = RetryStrategy(
                () =>
                {
                    using (HttpResponseMessage response = _client.GetAsync(version.DownloadUrl).Result)
                    {
                        response.EnsureSuccessStatusCode();

                        using (FileStream fs = File.Create(fileName))
                        {
                            response.Content.CopyToAsync(fs).Wait();
                        }
                    }
                },
                MAX_RETRIES
            );

            if (success)
            {
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
