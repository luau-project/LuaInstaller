using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LuaInstaller.Core
{
    public class VisualStudioCompiler : ICompiler
    {
        private string _buildDirectory;
        private readonly string _path;
        private readonly IDictionary<string, string> _defines;
        private readonly ICollection<string> _includes;
        private readonly ICollection<string> _sourceFiles;

        public VisualStudioCompiler(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;
            _defines = new Dictionary<string, string>();
            _includes = new List<string>();
            _sourceFiles = new List<string>();
        }

        public string BuildDirectory
        {
            get
            {
                return _buildDirectory;
            }
            set
            {
                _buildDirectory = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string DefaultObjectExtension
        {
            get
            {
                return ".obj";
            }
        }

        public void AddDefine(string name)
        {
            AddDefine(name, null);
        }

        public void AddDefine(string name, string value)
        {
            _defines[name] = value;
        }

        public void AddIncludeDirectory(string path)
        {
            _includes.Add(path);
        }

        public void AddSourceFile(string path)
        {
            _sourceFiles.Add(path);
        }

        private static string FormatDefine(KeyValuePair<string, string> keyValue)
        {
            string result;

            if (keyValue.Value == null)
            {
                result = string.Format("\"/D{0}\"", keyValue.Key);
            }
            else
            {
                result = string.Format("\"/D{0}={1}\"", keyValue.Key, keyValue.Value);
            }

            return result;
        }

        private static string FormatInclude(string path)
        {
            return string.Format("\"/I{0}\"", path);
        }

        private static string FormatSrcFile(string path)
        {
            return string.Format("\"{0}\"", path);
        }

        public int Execute()
        {
            int result = 1;

            ICollection<string> defines = new List<string>();
            foreach (KeyValuePair<string, string> keyValue in _defines)
            {
                defines.Add(FormatDefine(keyValue));
            }

            string arguments = string.Format(
                "/c /O2 /W3 {0} {1} {2}",
                string.Join(" ", defines.ToArray()),
                string.Join(" ", _includes.Select(FormatInclude).ToArray()),
                string.Join(" ", _sourceFiles.Select(FormatSrcFile).ToArray())
            );

            ProcessStartInfo compilePsi = new ProcessStartInfo();
            compilePsi.UseShellExecute = false;
            compilePsi.CreateNoWindow = true;
            compilePsi.WorkingDirectory = _buildDirectory;
            compilePsi.FileName = _path;
            compilePsi.Arguments = arguments;

            Process compileProcess = Process.Start(compilePsi);
            if (compileProcess != null)
            {
                compileProcess.WaitForExit();
                result = compileProcess.ExitCode;

                compileProcess.Dispose();
            }

            return result;
        }

        public void Reset()
        {
            _buildDirectory = null;
            _defines.Clear();
            _includes.Clear();
            _sourceFiles.Clear();
        }
    }
}
