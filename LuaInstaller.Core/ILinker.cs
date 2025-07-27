namespace LuaInstaller.Core
{
    public interface ILinker
    {
        string Path { get; }
        string BuildDirectory { get; set; }
        void AddLibPath(string path);
        void AddInputFile(string path);
        void AddLibInputFile(string path);
        void AddLinkerOption(LinkerOption option);

        int Execute();
        void Reset();
    }
}
