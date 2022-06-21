namespace LuaInstaller.Core
{
	public interface IWindowsSdkVersionLocator
	{
		WindowsSdkVersion[] GetVersions();
	}
}
