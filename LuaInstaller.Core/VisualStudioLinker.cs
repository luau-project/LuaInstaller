using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LuaInstaller.Core
{
    public sealed class VisualStudioLinker : ILinker
    {
        private string _buildDirectory;
        private readonly string _path;
        private readonly ICollection<LinkerOption> _options;
        private readonly ICollection<LinkerOption> _inputFiles;

        public VisualStudioLinker(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;

            _options = new List<LinkerOption>();
            _inputFiles = new List<LinkerOption>();
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

            if (option is VisualStudioInputFileLinkerOption)
            {
                _inputFiles.Add(option);
            }
            else
            {
                _options.Add(option);
            }
        }

        public int Execute()
        {
            int result = 1;

            string arguments = string.Format(
                "{0} {1}",
                string.Join(" ", _options.Select(FormatLinkerOption).ToArray()),
                string.Join(" ", _inputFiles.Select(FormatLinkerOption).ToArray())
            );

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
            _inputFiles.Clear();
        }
    }
}
