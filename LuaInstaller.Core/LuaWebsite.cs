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

        private const string LUA_DOWNLOAD_URL = "https://lua.org/ftp/";
        private static readonly HttpClient _client;
        private static readonly LuaVersion[] _empty;
        private static readonly LuaVersion _minimumSupportedVersion;

        static LuaWebsite()
        {
            _client = new HttpClient();
            _empty = new LuaVersion[0];
            _minimumSupportedVersion = new LuaVersion(5, 1, null);
        }

        private static string GetDownloadUrlForLuaVersion(LuaVersion version)
        {
            return string.Format("{0}lua-{1}.tar.gz", LUA_DOWNLOAD_URL, version.Version);
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
            SortedSet<LuaVersion> result = new SortedSet<LuaVersion>(LuaVersionComparers.Descending);

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

                    LuaVersion version = new LuaVersion(major, minor, build);

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

        [Obsolete("This method is deprecated. Use TryGetLatestVersion instead.")]
        public static LuaVersion GetLatestVersion()
        {
            return QueryVersions()[0];
        }

        public static LuaVersion FindVersion(string version)
        {
            LuaVersion[] versions = QueryVersions();
            return Array.Find(versions, v => v.Version == version);
        }

        /// <summary>
        /// Downloads the source code of Lua (<paramref name="version"/>)
        /// to directory <paramref name="workingDir"/>. The compressed archive
        /// is going to be extracted inside of the directory <paramref name="destDir"/>.
        /// </summary>
        /// <param name="destDir">Directory to extract the source code</param>
        /// <param name="workingDir">Directory to hold the compressed archive which contains the source code</param>
        /// <param name="version">The version of Lua</param>
        /// <returns>The path to the directory containing the source code</returns>
        /// <exception cref="DownloadAndExtractException"></exception>
        internal static string DownloadAndExtract(string destDir, string workingDir, LuaVersion version)
        {
            if (!Directory.Exists(destDir))
            {
                throw new DownloadAndExtractException("destDir does not exist");
            }

            if (!Directory.Exists(workingDir))
            {
                throw new DownloadAndExtractException("workingDir does not exist");
            }

            if (version == null)
            {
                throw new DownloadAndExtractException("version cannot be null");
            }

            if (LuaVersionComparers.Ascending.Compare(version, _minimumSupportedVersion) < 0)
            {
                throw new DownloadAndExtractException(string.Format("minimum supported version is {0}", _minimumSupportedVersion));
            }

            string fileName = null;

            try
            {
                fileName = Path.Combine(workingDir, string.Format("{0}.tar.gz", Guid.NewGuid().ToString("N")));
            }
            catch (Exception e)
            {
                throw new DownloadAndExtractException("Failed to create a filename for Lua's tarball", e);
            }

            bool success = RetryStrategy(
                () =>
                {
                    string downloadUrl = GetDownloadUrlForLuaVersion(version);

                    using (HttpResponseMessage response = _client.GetAsync(downloadUrl).Result)
                    {
                        response.EnsureSuccessStatusCode();

                        using (HttpContent content = response.Content)
                        using (FileStream fs = File.Create(fileName))
                        {
                            content.CopyToAsync(fs).Wait();
                        }
                    }
                },
                MAX_RETRIES
            );

            string sourcesDir = null;

            if (success)
            {
                try
                {
                    using (FileStream fs = File.OpenRead(fileName))
                    using (GZipInputStream gzs = new GZipInputStream(fs))
                    using (TarArchive tar = TarArchive.CreateInputTarArchive(gzs, null))
                    {
                        tar.ExtractContents(destDir);

                        sourcesDir = Path.Combine(destDir, "lua-" + version.Version);
                    }
                }
                catch (Exception e)
                {
                    throw new DownloadAndExtractException("Failed to extract Lua source code", e);
                }
            }
            else
            {
                throw new DownloadAndExtractException("Failed to download Lua source code");
            }

            if (!Directory.Exists(sourcesDir))
            {
                throw new DownloadAndExtractException("Directory containing the source code of Lua was not found");
            }

            return sourcesDir;
        }
    }
}
