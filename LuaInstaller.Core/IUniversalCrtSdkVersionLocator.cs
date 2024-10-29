namespace LuaInstaller.Core
{
    public interface IUniversalCrtSdkVersionLocator
    {
        WindowsSdkVersion[] GetVersions();
    }
}
