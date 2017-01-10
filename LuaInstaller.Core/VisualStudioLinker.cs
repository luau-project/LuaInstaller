using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LuaInstaller.Core
{
    public class VisualStudioLinker : ILinker
    {
        private string _buildDirectory;
        private string _outputFile;
        private bool _dll;
        private readonly string _path;
        private readonly ICollection<string> _libPaths;
        private readonly ICollection<string> _inputFiles;

        public VisualStudioLinker(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;

            _libPaths = new List<string>();
            _inputFiles = new List<string>();
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

        public string OutputFile
        {
            get
            {
                return _outputFile;
            }

            set
            {
                _outputFile = value;
            }
        }

        public bool Dll
        {
            get
            {
                return _dll;
            }

            set
            {
                _dll = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

        private static string FormatInputFile(string path)
        {
            return string.Format("\"{0}\"", path);
        }

        public void AddInputFile(string path)
        {
            _inputFiles.Add(path);
        }

        private static string FormatLibPath(string path)
        {
            return string.Format("\"/LIBPATH:{0}\"", path);
        }

        public void AddLibPath(string path)
        {
            _libPaths.Add(path);
        }

        public int Execute()
        {
            int result = 1;
            
            string arguments = string.Format(
                _dll ? "/DLL \"/OUT:{0}\" {1} {2}" : "\"/OUT:{0}\" {1} {2}",
                _outputFile,
                string.Join(" ", _libPaths.Select(FormatLibPath).ToArray()),
                string.Join(" ", _inputFiles.Select(FormatInputFile).ToArray())
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
            _dll = false;
            _outputFile = null;
            _libPaths.Clear();
            _inputFiles.Clear();
        }
    }
}
