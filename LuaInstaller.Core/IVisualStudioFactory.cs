namespace LuaInstaller.Core
{
    public interface IVisualStudioFactory
    {
        VisualStudio Create(VisualStudioVersion version, Architecture arch);
    }
}
