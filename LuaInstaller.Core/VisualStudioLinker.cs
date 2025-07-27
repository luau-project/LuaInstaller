using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LuaInstaller.Core
{
    public sealed class VisualStudioLinker : ILinker
    {
        private string _buildDirectory;
        private readonly string _path;
        private readonly IDictionary<int, Queue<LinkerOption>> _options;

        public VisualStudioLinker(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;
            _options = new Dictionary<int, Queue<LinkerOption>>();
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

        private static string FormatLinkerOption(LinkerOption option)
        {
            return string.Format("\"{0}\"", option.ToString());
        }

        public void AddInputFile(string path)
        {
            AddLinkerOption(new VisualStudioInputFileLinkerOption(path));
        }

        public void AddLibInputFile(string path)
        {
            AddLinkerOption(new VisualStudioLibInputFileLinkerOption(path));
        }

        public void AddLibPath(string path)
        {
            AddLinkerOption(new VisualStudioLibraryDirectoryLinkerOption(path));
        }

        public void AddLinkerOption(LinkerOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException();
            }
            Queue<LinkerOption> queue;
            if (!_options.TryGetValue(option.CommandLineSortOrder, out queue))
            {
                queue = new Queue<LinkerOption>();
                _options.Add(option.CommandLineSortOrder, queue);
            }
            queue.Enqueue(option);
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
                argumentsBuilder.Append(string.Join(" ", _options[key].Select(FormatLinkerOption)));
                if (i < orderedKeys.Length - 1)
                {
                    argumentsBuilder.Append(" ");
                }
            }
            string arguments = argumentsBuilder.ToString();

            ProcessStartInfo linkPsi = new ProcessStartInfo();
            linkPsi.UseShellExecute = false;
            linkPsi.CreateNoWindow = true;
            linkPsi.WorkingDirectory = _buildDirectory;
            linkPsi.FileName = _path;
            linkPsi.Arguments = arguments;

            Process linkProcess = Process.Start(linkPsi);
            if (linkProcess != null)
            {
                linkProcess.WaitForExit();
                result = linkProcess.ExitCode;

                linkProcess.Dispose();
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
