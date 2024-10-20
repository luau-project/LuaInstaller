using System;
using System.IO;
using System.Linq;

namespace LuaInstaller.Core
{
    public sealed class InstallationManager
    {
        private readonly ICompiler _compiler;
        private readonly ILinker _linker;
        private readonly string _objExtension;
        
        public event EventHandler<InstallationProgressEventArgs> InstallationProgressChanged;
        private void OnInstallationProgressChanged(InstallationProgress progress)
        {
            if (InstallationProgressChanged != null)
            {
                InstallationProgressChanged(this, new InstallationProgressEventArgs(progress));
            }
        }

        public InstallationManager(ICompiler compiler, ILinker linker)
        {
            if (compiler == null)
            {
                throw new ArgumentNullException("compiler");
            }

            if (linker == null)
            {
                throw new ArgumentNullException("linker");
            }

            _compiler = compiler;
            _linker = linker;
            _objExtension = _compiler.DefaultObjectExtension;
        }

        private void LinkDll(string srcDir, string outputFile, VisualStudio vs, WindowsSdk winsdk)
        {
            try
            {
                _linker.BuildDirectory = srcDir;
                _linker.OutputFile = outputFile;
                _linker.Dll = true;

                foreach (string libPath in vs.LibPathDirectories)
                {
                    _linker.AddLibPath(libPath);
                }

                foreach (string libPath in winsdk.LibPathDirectories)
                {
                    _linker.AddLibPath(libPath);
                }

                DirectoryInfo srcDirInfo = new DirectoryInfo(srcDir);
                foreach (string srcFile in srcDirInfo.EnumerateFiles("*" + _objExtension).Select(f => f.FullName))
                {
                    _linker.AddInputFile(srcFile);
                }

                if (_linker.Execute() != 0)
                {
                    throw new LinkDllException();
                }

                OnInstallationProgressChanged(Core.InstallationProgress.LinkDll);
            }
            finally
            {
                _linker.Reset();
            }
        }
        private void BuildDll(string srcDir, string outputFile, VisualStudio vs, WindowsSdk winsdk)
        {
            string buildDir = Path.Combine(
                Path.GetTempPath(), "li-build-dll-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                _compiler.AddDefine("LUA_BUILD_AS_DLL");

                foreach (string inc in vs.IncludeDirectories)
                {
                    _compiler.AddIncludeDirectory(inc);
                }

                foreach (string inc in winsdk.IncludeDirectories)
                {
                    _compiler.AddIncludeDirectory(inc);
                }

                DirectoryInfo srcDirInfo = new DirectoryInfo(srcDir);
                foreach (string srcFile in srcDirInfo.EnumerateFiles("*.c").Where(f => f.Name != "luac.c" && f.Name != "lua.c").Select(f => f.FullName))
                {
                    _compiler.AddSourceFile(srcFile);
                }

                Directory.CreateDirectory(buildDir);
                
                _compiler.BuildDirectory = buildDir;
                _compiler.AddIncludeDirectory(buildDir);
                
                if (_compiler.Execute() == 0)
                {
                    OnInstallationProgressChanged(Core.InstallationProgress.CompileDll);

                    LinkDll(buildDir, outputFile, vs, winsdk);
                }
                else
                {
                    throw new CompileDllException();
                }
            }
            catch (BuildDllException ex)
            {
#pragma warning disable CA2200
                throw ex;
#pragma warning restore CA2200
            }
            catch (Exception ex)
            {
                throw new BuildDllException("Failed to build dll", ex);
            }
            finally
            {
                _compiler.Reset();

                if (Directory.Exists(buildDir))
                {
                    Directory.Delete(buildDir, true);
                }
            }
        }
        
        private void LinkInterpreter(string srcDir, string luaLibPath, string outputFile, VisualStudio vs, WindowsSdk winsdk)
        {
            try
            {
                _linker.BuildDirectory = srcDir;
                _linker.OutputFile = outputFile;

                foreach (string libPath in vs.LibPathDirectories)
                {
                    _linker.AddLibPath(libPath);
                }

                foreach (string libPath in winsdk.LibPathDirectories)
                {
                    _linker.AddLibPath(libPath);
                }

                _linker.AddInputFile(Path.Combine(srcDir, "lua" + _objExtension));
                _linker.AddInputFile(luaLibPath);

                if (_linker.Execute() != 0)
                {
                    throw new LinkInterpreterException();
                }

                OnInstallationProgressChanged(Core.InstallationProgress.LinkInterpreter);
            }
            finally
            {
                _linker.Reset();
            }
        }
        private void BuildInterpreter(string srcDir, string luaLibPath, string outputFile, VisualStudio vs, WindowsSdk winsdk)
        {
            string buildDir = Path.Combine(
                Path.GetTempPath(), "li-interpreter-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                foreach (string inc in vs.IncludeDirectories)
                {
                    _compiler.AddIncludeDirectory(inc);
                }

                foreach (string inc in winsdk.IncludeDirectories)
                {
                    _compiler.AddIncludeDirectory(inc);
                }

                _compiler.AddSourceFile(Path.Combine(srcDir, "lua.c"));

                Directory.CreateDirectory(buildDir);
                
                _compiler.BuildDirectory = buildDir;
                _compiler.AddIncludeDirectory(buildDir);
                
                if (_compiler.Execute() == 0)
                {
                    OnInstallationProgressChanged(Core.InstallationProgress.CompileInterpreter);
                    LinkInterpreter(buildDir, luaLibPath, outputFile, vs, winsdk);
                }
                else
                {
                    throw new CompileInterpreterException();
                }
            }
            catch (BuildInterpreterException ex)
            {
#pragma warning disable CA2200
                throw ex;
#pragma warning restore CA2200
            }
            catch (Exception ex)
            {
                throw new BuildInterpreterException("Failed to build dll", ex);
            }
            finally
            {
                _compiler.Reset();

                if (Directory.Exists(buildDir))
                {
                    Directory.Delete(buildDir, true);
                }
            }
        }

        private void LinkCompiler(string srcDir, string outputFile, VisualStudio vs, WindowsSdk winsdk)
        {
            try
            {
                _linker.BuildDirectory = srcDir;
                _linker.OutputFile = outputFile;

                foreach (string libPath in vs.LibPathDirectories)
                {
                    _linker.AddLibPath(libPath);
                }

                foreach (string libPath in winsdk.LibPathDirectories)
                {
                    _linker.AddLibPath(libPath);
                }

                DirectoryInfo srcDirInfo = new DirectoryInfo(srcDir);
                foreach (string srcFile in srcDirInfo.EnumerateFiles("*" + _objExtension).Select(f => f.FullName))
                {
                    _linker.AddInputFile(srcFile);
                }

                if (_linker.Execute() != 0)
                {
                    throw new LinkCompilerException();
                }

                OnInstallationProgressChanged(Core.InstallationProgress.LinkCompiler);
            }
            finally
            {
                _linker.Reset();
            }
        }
        private void BuildCompiler(string srcDir, string outputFile, VisualStudio vs, WindowsSdk winsdk)
        {
            string buildDir = Path.Combine(
                Path.GetTempPath(), "li-build-compiler-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                foreach (string inc in vs.IncludeDirectories)
                {
                    _compiler.AddIncludeDirectory(inc);
                }

                foreach (string inc in winsdk.IncludeDirectories)
                {
                    _compiler.AddIncludeDirectory(inc);
                }

                DirectoryInfo srcDirInfo = new DirectoryInfo(srcDir);
                foreach (string srcFile in srcDirInfo.EnumerateFiles("*.c").Where(f => f.Name != "lua.c").Select(f => f.FullName))
                {
                    _compiler.AddSourceFile(srcFile);
                }

                Directory.CreateDirectory(buildDir);

                _compiler.BuildDirectory = buildDir;
                _compiler.AddIncludeDirectory(buildDir);

                if (_compiler.Execute() == 0)
                {
                    OnInstallationProgressChanged(Core.InstallationProgress.CompileCompiler);
                    LinkCompiler(buildDir, outputFile, vs, winsdk);
                }
                else
                {
                    throw new CompileCompilerException();
                }
            }
            catch (BuildCompilerException ex)
            {
#pragma warning disable CA2200
                throw ex;
#pragma warning restore CA2200
            }
            catch (Exception ex)
            {
                throw new BuildCompilerException("Failed to build dll", ex);
            }
            finally
            {
                _compiler.Reset();

                if (Directory.Exists(buildDir))
                {
                    Directory.Delete(buildDir, true);
                }
            }
        }

        /// <summary>
        /// Performs a hard-and-deep-copy from the <paramref name="workDir"/>
        /// to the <paramref name="destDir"/>. Here, hard means that colliding
        /// directory and file names are deleted / overwritten.
        /// </summary>
        /// <param name="workDir">Work directory of the built artifacts</param>
        /// <param name="destDir">Destination directory chosen by the user</param>
        private void InstallOnDestDirCore(string workDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string entry in Directory.GetFileSystemEntries(workDir))
            {
                string entryName = Path.GetFileName(entry);
                string expectedDestSubentry = Path.Combine(destDir, entryName);
                
                if (Directory.Exists(entry))
                {
                    if (File.Exists(expectedDestSubentry))
                    {
                        // Normally, this is potentially a dangerous operation
                        // on random files. However, assuming
                        // the unpacked tarball is legit
                        // and the user did not pick a sensitive
                        // folder (system-managed, e.g., 'C:\Windows' and such),
                        // it should be safe.
                        File.Delete(expectedDestSubentry);
                    }

                    InstallOnDestDirCore(entry, expectedDestSubentry);
                }
                else if (Directory.Exists(expectedDestSubentry))
                {
                    // Normally, this is potentially a dangerous operation
                    // on random files. However, assuming
                    // the unpacked tarball is legit
                    // and the user did not pick a sensitive
                    // folder (system-managed, e.g., 'C:\Windows' and such),
                    // it should be safe.
                    Directory.Delete(expectedDestSubentry, true);
                }
                else
                {
                    // Normally, this is potentially a dangerous operation
                    // on random files. However, assuming
                    // the unpacked tarball is legit
                    // and the user did not pick a sensitive
                    // folder (system-managed, e.g., 'C:\Windows' and such),
                    // it should be safe.
                    File.Copy(entry, expectedDestSubentry, true);
                }
            }
        }

        /// <summary>
        /// Performs a hard-and-deep-copy from the <paramref name="workDir"/>
        /// to the <paramref name="destDir"/>. Here, hard means that colliding
        /// directory and file names are deleted / overwritten.
        /// </summary>
        /// <param name="workDir">Work directory of the built artifacts</param>
        /// <param name="destDir">Destination directory chosen by the user</param>
        private void InstallOnDestDir(LuaDestinationDirectory workDir, LuaDestinationDirectory destDir)
        {
            try
            {
                InstallOnDestDirCore(workDir.Path, destDir.Path);
                OnInstallationProgressChanged(InstallationProgress.InstallOnDestDir);
            }
            catch (Exception ex)
            {
                throw new InstallOnDestDirException("Unable to install Lua on destination directory", ex);
            }
        }
        
        private void CreatePkgConfigFile(LuaDestinationDirectory destDir, LuaVersion version, LuaGeneratedBinaries generatedBinaries)
        {
            try
            {
                PkgConfigOutputInfo pkgConfigOutput = new PkgConfigOutputInfo(version, generatedBinaries, destDir);
                
                if (pkgConfigOutput.WritePkgConfigFile())
                {
                    OnInstallationProgressChanged(InstallationProgress.CreatePkgConfigFile);
                }
                else
                {
                    throw new Exception("Failed to create pkg-config file");
                }
            }
            catch (Exception ex)
            {
                throw new CreatePkgConfigFileException("Unable to create pkg-config file for Lua", ex);
            }
        }

        private void SetEnvironmentVariables(LuaDestinationDirectory destDir, EnvironmentVariableTarget? variableTarget)
        {
            if (variableTarget.HasValue)
            {
                EnvironmentVariableTarget target = variableTarget.Value;

                try
                {
                    string path = Environment.GetEnvironmentVariable("PATH", target);

                    string pathValue;

                    if (path == null)
                    {
                        pathValue = destDir.Bin;
                    }
                    else
                    {
                        string[] pathDirs = path.Split(Path.PathSeparator);

                        if (Array.Exists(pathDirs, p => p.Trim().Equals(destDir.Bin, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            pathValue = path;
                        }
                        else
                        {
                            pathValue = destDir.Bin + Path.PathSeparator + path;
                        }
                    }

                    Environment.SetEnvironmentVariable("PATH", pathValue, target);

                    Environment.SetEnvironmentVariable("LUA_DIR", destDir.Path, target);
                    Environment.SetEnvironmentVariable("LUA_INC", destDir.Include, target);
                    Environment.SetEnvironmentVariable("LUA_LIB", destDir.Lib, target);
                    Environment.SetEnvironmentVariable("LUA_BIN", destDir.Bin, target);

                    OnInstallationProgressChanged(InstallationProgress.EnvironmentVariables);
                }
                catch (Exception ex)
                {
                    throw new EnvironmentVariablesException("Unable to set environment variables", ex);
                }
            }
        }

        public void Build(string version, string luaDestDir, VisualStudio vs, WindowsSdk winsdk, EnvironmentVariableTarget? variableTarget = null)
        {
            Build(LuaWebsite.FindVersion(version), luaDestDir, vs, winsdk, variableTarget);
        }
        public void Build(LuaVersion version, string luaDestDir, VisualStudio vs, WindowsSdk winsdk, EnvironmentVariableTarget? variableTarget = null)
        {
            string tempPath = Path.GetTempPath();

            string luaDownloadDir = Path.Combine(
                tempPath,
                "li-download-" + Guid.NewGuid().ToString("N")
            );

            string luaExtractionDir = Path.Combine(
                tempPath,
                "li-extraction-" + Guid.NewGuid().ToString("N")
            );

            string luaWorkDir = Path.Combine(
                tempPath,
                "li-workdir-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                Directory.CreateDirectory(luaDownloadDir);
                Directory.CreateDirectory(luaExtractionDir);
                Directory.CreateDirectory(luaWorkDir);

                string luaSourcesDir = LuaWebsite.DownloadAndExtract(luaDownloadDir, luaExtractionDir, version);
                OnInstallationProgressChanged(Core.InstallationProgress.Download);

                LuaSourcesDirectory sourcesDir = new LuaSourcesDirectory(luaSourcesDir);
                LuaDestinationDirectory workDir = new LuaDestinationDirectory(luaWorkDir);
                LuaGeneratedBinaries generatedBinaries = new LuaGeneratedBinaries(version);
                
                workDir.CreateFrom(sourcesDir);

                BuildDll(
                    sourcesDir.Src,
                    Path.Combine(workDir.Lib, generatedBinaries.DllName),
                    vs,
                    winsdk
                );
                BuildInterpreter(
                    sourcesDir.Src,
                    Path.Combine(workDir.Lib, generatedBinaries.ImportLibName), 
                    Path.Combine(workDir.Bin, generatedBinaries.InterpreterName),
                    vs,
                    winsdk
                );
                File.Copy(
                    Path.Combine(workDir.Lib, generatedBinaries.DllName),
                    Path.Combine(workDir.Bin, generatedBinaries.DllName),
                    true
                );
                BuildCompiler(
                    sourcesDir.Src,
                    Path.Combine(workDir.Bin, generatedBinaries.CompilerName),
                    vs,
                    winsdk
                );

                if (File.Exists(Path.Combine(workDir.Lib, generatedBinaries.DllName)))
                {
                    File.Delete(
                        Path.Combine(workDir.Lib, generatedBinaries.DllName)
                    );
                }

                LuaDestinationDirectory destDir = new LuaDestinationDirectory(luaDestDir);
                InstallOnDestDir(workDir, destDir);
                CreatePkgConfigFile(destDir, version, generatedBinaries);
                SetEnvironmentVariables(destDir, variableTarget);
            }
            finally
            {
                if (Directory.Exists(luaDownloadDir))
                {
                    Directory.Delete(luaDownloadDir, true);
                }

                if (Directory.Exists(luaExtractionDir))
                {
                    Directory.Delete(luaExtractionDir, true);
                }

                if (Directory.Exists(luaWorkDir))
                {
                    Directory.Delete(luaWorkDir, true);
                }
            }

            OnInstallationProgressChanged(InstallationProgress.Finished);
        }
    }
}
