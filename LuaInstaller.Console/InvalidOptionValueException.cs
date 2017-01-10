namespace LuaInstaller.Console
{
    public class InvalidOptionValueException : InvalidOptionException
    {
        public InvalidOptionValueException(string message) : base(message)
        {
        }
    }
}
