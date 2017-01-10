namespace LuaInstaller.Console
{
    public interface ICliArguments
    {
        void Process(string[] args, int index);
    }
}
