using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LuaInstaller.Core
{
    public sealed class VisualStudioCompiler : ICompiler
    {
        private string _buildDirectory;
        private readonly string _path;
        private readonly ICollection<CompilerOption> _options;
        private readonly ICollection<CompilerOption> _sourceFiles;

        public VisualStudioCompiler(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            _path = path;
            _options = new List<CompilerOption>();
            _sourceFiles = new List<CompilerOption>();
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

            if (option is VisualStudioSourceFileCompilerOption)
            {
                _sourceFiles.Add(option);
            }
            else
            {
                _options.Add(option);
            }
        }

        private static string FormatCompilerOption(CompilerOption option)
        {
            return string.Format("\"{0}\"", option.ToString());
        }

        public int Execute()
        {
            int result = 1;

            string arguments = string.Format(
                "{0} {1}",
                string.Join(" ", _options.Select(FormatCompilerOption).ToArray()),
                string.Join(" ", _sourceFiles.Select(FormatCompilerOption).ToArray())
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
            _options.Clear();
            _sourceFiles.Clear();
        }
    }
}
