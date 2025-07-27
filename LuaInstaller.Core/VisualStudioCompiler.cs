using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LuaInstaller.Core
{
    public sealed class VisualStudioCompiler : ICompiler
    {
        private string _buildDirectory;
        private readonly string _path;
        private readonly IDictionary<int, Queue<CompilerOption>> _options;

        public VisualStudioCompiler(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;
            _options = new Dictionary<int, Queue<CompilerOption>>();
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
            AddCompilerOption(new VisualStudioPreprocessorDefinitionCompilerOption(name));
        }

        public void AddDefine(string name, string value)
        {
            AddCompilerOption(new VisualStudioPreprocessorDefinitionCompilerOption(name, value));
        }

        public void AddIncludeDirectory(string path)
        {
            AddCompilerOption(new VisualStudioIncludeDirectoryCompilerOption(path));
        }

        public void AddSourceFile(string path)
        {
            AddCompilerOption(new VisualStudioSourceFileCompilerOption(path));
        }

        public void AddCompilerOption(CompilerOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException();
            }
            Queue<CompilerOption> queue;
            if (!_options.TryGetValue(option.CommandLineSortOrder, out queue))
            {
                queue = new Queue<CompilerOption>();
                _options.Add(option.CommandLineSortOrder, queue);
            }
            queue.Enqueue(option);
        }

        private static string FormatCompilerOption(CompilerOption option)
        {
            return string.Format("\"{0}\"", option.ToString());
        }

        public int Execute()
        {
            int result = 1;

            int key;
            ICollection<int> keys = _options.Keys;
            int[] orderedKeys = new int[keys.Count];
            keys.CopyTo(orderedKeys, 0);
            Array.Sort<int>(orderedKeys);
            StringBuilder argumentsBuilder = new StringBuilder(Math.Max(2 * orderedKeys.Length - 1, 4));
            for (int i = 0; i < orderedKeys.Length; i++)
            {
                key = orderedKeys[i];
                argumentsBuilder.Append(string.Join(" ", _options[key].Select(FormatCompilerOption)));
                if (i < orderedKeys.Length - 1)
                {
                    argumentsBuilder.Append(" ");
                }
            }
            string arguments = argumentsBuilder.ToString();

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
            _options.Clear();
        }
    }
}
