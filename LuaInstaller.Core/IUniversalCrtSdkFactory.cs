namespace LuaInstaller.Core
{
    public interface IUniversalCrtSdkFactory
    {
        WindowsSdk Create(WindowsSdkVersion version, Architecture arch);
    }
}
