namespace LuaInstaller.Core
{
    public interface ILinker
    {
        string Path { get; }
        string BuildDirectory { get; set; }
        string OutputFile { get; set; }
        bool Dll { get; set; }
        void AddLibPath(string path);
        void AddInputFile(string path);
        int Execute();
        void Reset();
    }
}
