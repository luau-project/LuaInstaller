using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        private void SetupCompilerSharedConfiguration()
        {
            if (_compiler is VisualStudioCompiler)
            {
                // /nologo
                _compiler.AddCompilerOption(new VisualStudioSuppressStartupBannerCompilerOption());

                // /c
                _compiler.AddCompilerOption(new VisualStudioCompileOnlyCompilerOption());

                // /MD
                _compiler.AddCompilerOption(new VisualStudioMultithreadedDLLRuntimeLibraryCompilerOption(false));

                // /O2
                _compiler.AddCompilerOption(new VisualStudioO2CompilerOption());

                // /W3
                _compiler.AddCompilerOption(new VisualStudioW3CompilerOption());

                // /D_CRT_SECURE_NO_DEPRECATE
                _compiler.AddDefine("_CRT_SECURE_NO_DEPRECATE");
            }
        }

        private void SetupLinkerSharedConfiguration()
        {
            if (_linker is VisualStudioLinker)
            {
                // /nologo
                _linker.AddLinkerOption(new  VisualStudioSuppressStartupBannerLinkerOption());
            }
        }

        private AbstractLuaCompatibility GetLuaCompatibility(string srcDir)
        {
            bool result = false;
            string luaCompat = null;

            string srcMakefile = Path.Combine(srcDir, "Makefile");

            if (File.Exists(srcMakefile))
            {
                using (FileStream fileStream = File.OpenRead(srcMakefile))
                using (StreamReader Makefile = new StreamReader(fileStream))
                {
                    string line = null;
                    Match match = null;
                    Regex regex = new Regex(@"\-D(LUA_COMPAT_[a-zA-Z0-9_]+)\b");

                    while (!(result || Makefile.EndOfStream))
                    {
                        line = Makefile.ReadLine();

                        match = regex.Match(line);

                        if (match.Success)
                        {
                            luaCompat = match.Groups[1].Value;

                            result = true;
                        }
                    }
                }
            }

            AbstractLuaCompatibility compatibility;
            if (result)
            {
                compatibility = new LuaCompatibility(luaCompat);
            }
            else
            {
                compatibility = new LuaNoCompatibility();
            }
            return compatibility;
        }

        private void LinkDll(string srcDir, string outputFile, VisualStudio vs, WindowsSdk winsdk)
        {
            try
            {
                SetupLinkerSharedConfiguration();

                _linker.BuildDirectory = srcDir;

                if (_linker is VisualStudioLinker)
                {
                    _linker.AddLinkerOption(new VisualStudioOutputFileLinkerOption(outputFile));
                    _linker.AddLinkerOption(new VisualStudioDLLLinkerOption());
                }

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

        private void BuildDll(string executionDir, string srcDir, string outputFile, VisualStudio vs, WindowsSdk winsdk, AbstractLuaCompatibility luaCompat)
        {
            string buildDir = Path.Combine(
                executionDir, "li-build-dll-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                SetupCompilerSharedConfiguration();

                luaCompat.TryAddDefine(_compiler);

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
                _compiler.AddIncludeDirectory(srcDir);

                if (_compiler.Execute() == 0)
                {
                    OnInstallationProgressChanged(InstallationProgress.CompileDll);

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
                SetupLinkerSharedConfiguration();

                _linker.BuildDirectory = srcDir;

                if (_linker is VisualStudioLinker)
                {
                    _linker.AddLinkerOption(new VisualStudioOutputFileLinkerOption(outputFile));
                }

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

                OnInstallationProgressChanged(InstallationProgress.LinkInterpreter);
            }
            finally
            {
                _linker.Reset();
            }
        }

        private void BuildInterpreter(string executionDir, string srcDir, string luaLibPath, string outputFile, VisualStudio vs, WindowsSdk winsdk, AbstractLuaCompatibility luaCompat)
        {
            string buildDir = Path.Combine(
                executionDir, "li-interpreter-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                SetupCompilerSharedConfiguration();

                luaCompat.TryAddDefine(_compiler);

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
                _compiler.AddIncludeDirectory(srcDir);

                if (_compiler.Execute() == 0)
                {
                    OnInstallationProgressChanged(InstallationProgress.CompileInterpreter);
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
                SetupLinkerSharedConfiguration();

                _linker.BuildDirectory = srcDir;

                if (_linker is VisualStudioLinker)
                {
                    _linker.AddLinkerOption(new VisualStudioOutputFileLinkerOption(outputFile));
                }

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

        private void BuildCompiler(string executionDir, string srcDir, string outputFile, VisualStudio vs, WindowsSdk winsdk, AbstractLuaCompatibility luaCompat)
        {
            string buildDir = Path.Combine(
                executionDir, "li-build-compiler-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                SetupCompilerSharedConfiguration();

                luaCompat.TryAddDefine(_compiler);

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
                _compiler.AddIncludeDirectory(srcDir);

                if (_compiler.Execute() == 0)
                {
                    OnInstallationProgressChanged(InstallationProgress.CompileCompiler);
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

        private void CreatePkgConfigFile(LuaDestinationDirectory destDir, LuaVersion version, LuaGeneratedBinaries generatedBinaries, AbstractLuaCompatibility luaCompat)
        {
            try
            {
                PkgConfigOutputInfo pkgConfigOutput = new PkgConfigOutputInfo(version, generatedBinaries, destDir, luaCompat);

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

                    string pathValue = null;
                    bool foundOnPath = false;

                    if (path == null)
                    {
                        pathValue = destDir.Bin;
                    }
                    else
                    {
                        string[] pathDirs = path.Split(Path.PathSeparator);
                        foundOnPath = Array.Exists(pathDirs, p => p.Trim().Equals(destDir.Bin, StringComparison.InvariantCultureIgnoreCase));

                        if (!foundOnPath)
                        {
                            pathValue = destDir.Bin + Path.PathSeparator + path;
                        }
                    }

                    if (!foundOnPath)
                    {
                        Environment.SetEnvironmentVariable("PATH", pathValue, target);
                    }

                    OnInstallationProgressChanged(InstallationProgress.EnvironmentVariables);
                }
                catch (Exception ex)
                {
                    throw new EnvironmentVariablesException("Unable to set environment variables", ex);
                }
            }
        }

        [Obsolete("This method was renamed. Use ExecuteInstall instead.")]
        public void Build(string version, string luaDestDir, VisualStudio vs, WindowsSdk winsdk, EnvironmentVariableTarget? variableTarget = null)
        {
            ExecuteInstall(version, luaDestDir, vs, winsdk, variableTarget);
        }

        [Obsolete("This method was renamed. Use ExecuteInstall instead.")]
        public void Build(LuaVersion version, string luaDestDir, VisualStudio vs, WindowsSdk winsdk, EnvironmentVariableTarget? variableTarget = null)
        {
            ExecuteInstall(version, luaDestDir, vs, winsdk, variableTarget);
        }

        public void ExecuteInstall(string version, string luaDestDir, VisualStudio vs, WindowsSdk winsdk, EnvironmentVariableTarget? variableTarget = null)
        {
            ExecuteInstall(LuaWebsite.FindVersion(version), luaDestDir, vs, winsdk, variableTarget);
        }

        public void ExecuteInstall(LuaVersion version, string luaDestDir, VisualStudio vs, WindowsSdk winsdk, EnvironmentVariableTarget? variableTarget = null)
        {
            string tempPath = Path.GetTempPath();

            string executionDir = Path.Combine(
                tempPath,
                "li-execution-" + Guid.NewGuid().ToString("N")
            );

            try
            {
                string luaDownloadDir = Path.Combine(
                    executionDir,
                    "li-download-" + Guid.NewGuid().ToString("N")
                );

                string luaExtractionDir = Path.Combine(
                    executionDir,
                    "li-extraction-" + Guid.NewGuid().ToString("N")
                );

                string luaWorkDir = Path.Combine(
                    executionDir,
                    "li-workdir-" + Guid.NewGuid().ToString("N")
                );

                Directory.CreateDirectory(executionDir);
                Directory.CreateDirectory(luaDownloadDir);
                Directory.CreateDirectory(luaExtractionDir);
                Directory.CreateDirectory(luaWorkDir);

                string luaSourcesDir = LuaWebsite.DownloadAndExtract(luaDownloadDir, luaExtractionDir, version);
                OnInstallationProgressChanged(InstallationProgress.Download);

                LuaSourcesDirectory sourcesDir = new LuaSourcesDirectory(luaSourcesDir);
                LuaDestinationDirectory workDir = new LuaDestinationDirectory(luaWorkDir);
                LuaGeneratedBinaries generatedBinaries = new LuaGeneratedBinaries(version);

                AbstractLuaCompatibility luaCompat = GetLuaCompatibility(sourcesDir.Src);

                workDir.CreateFrom(sourcesDir);

                BuildDll(
                    executionDir,
                    sourcesDir.Src,
                    Path.Combine(workDir.Lib, generatedBinaries.DllName),
                    vs,
                    winsdk,
                    luaCompat
                );
                BuildInterpreter(
                    executionDir,
                    sourcesDir.Src,
                    Path.Combine(workDir.Lib, generatedBinaries.ImportLibName), 
                    Path.Combine(workDir.Bin, generatedBinaries.InterpreterName),
                    vs,
                    winsdk,
                    luaCompat
                );
                File.Copy(
                    Path.Combine(workDir.Lib, generatedBinaries.DllName),
                    Path.Combine(workDir.Bin, generatedBinaries.DllName),
                    true
                );
                BuildCompiler(
                    executionDir,
                    sourcesDir.Src,
                    Path.Combine(workDir.Bin, generatedBinaries.CompilerName),
                    vs,
                    winsdk,
                    luaCompat
                );

                if (File.Exists(Path.Combine(workDir.Lib, generatedBinaries.DllName)))
                {
                    File.Delete(
                        Path.Combine(workDir.Lib, generatedBinaries.DllName)
                    );
                }

                LuaDestinationDirectory destDir = new LuaDestinationDirectory(luaDestDir);
                InstallOnDestDir(workDir, destDir);
                CreatePkgConfigFile(destDir, version, generatedBinaries, luaCompat);
                SetEnvironmentVariables(destDir, variableTarget);
            }
            finally
            {
                try
                {
                    if (executionDir != null && Directory.Exists(executionDir))
                    {
                        Directory.Delete(executionDir, true);
                    }
                }
                catch
                {

                }
            }

            OnInstallationProgressChanged(InstallationProgress.Finished);
        }
    }
}
