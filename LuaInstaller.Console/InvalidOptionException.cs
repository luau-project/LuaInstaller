namespace LuaInstaller.Console
{
    public class InvalidOptionException : CliArgumentsException
    {
        public InvalidOptionException(string message) : base(message)
        {
        }
    }
}
