namespace Vlingo
{
    public interface IDispatcher
    {
        void Close();
        void Execute(IMailbox mailbox);
        bool RequiresExecutionNotification();
    }
}