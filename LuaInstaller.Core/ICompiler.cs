namespace LuaInstaller.Core
{
    public interface ICompiler
    {
        string Path { get; }
        string BuildDirectory { get; set; }
        string DefaultObjectExtension { get; }
        void AddDefine(string name);
        void AddDefine(string name, string value);
        void AddIncludeDirectory(string path);
        void AddSourceFile(string path);
        int Execute();
        void Reset();
    }
}