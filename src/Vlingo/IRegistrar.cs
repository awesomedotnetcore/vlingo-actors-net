namespace Vlingo
{
    public interface IRegistrar
    {
        void Register(string name, bool isDefault, IMailboxProvider mailboxProvider);
        void Register(string name, bool isDefault, ILoggerProvider loggerProvider);
    }
}