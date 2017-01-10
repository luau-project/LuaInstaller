namespace LuaInstaller.Core
{
    public interface IWindowsSdkFactory
    {
        WindowsSdk Create(WindowsSdkVersion version, Architecture arch);
    }
}
