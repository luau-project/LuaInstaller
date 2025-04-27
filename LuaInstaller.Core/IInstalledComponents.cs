namespace LuaInstaller.Core
{
    public interface IInstalledComponents
    {
        IVisualStudioEnumeration AllVisualStudioX86();
        IWindowsSdkEnumeration AllWindowsSdkX86();

        IVisualStudioEnumeration AllVisualStudioX64();
        IWindowsSdkEnumeration AllWindowsSdkX64();

        IVisualStudioEnumeration AllVisualStudioARM64();
        IWindowsSdkEnumeration AllWindowsSdkARM64();

        IVisualStudioEnumeration AllVisualStudioByArch(Architecture arch);
        IWindowsSdkEnumeration AllWindowsSdkByArch(Architecture arch);
    }
}
