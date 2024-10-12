namespace LuaInstaller.Core
{
    public enum InstallationProgress
    {
        None,
        Download,
        CompileDll,
        LinkDll,
        CompileInterpreter,
        LinkInterpreter,
        CompileCompiler,
        LinkCompiler,
        CreatePkgConfigFile,
        InstallOnDestDir,
        EnvironmentVariables,
        Finished
    }
}
