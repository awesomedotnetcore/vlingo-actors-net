namespace Vlingo
{
    public interface IMailbox : IRunnable
    {
        void Close();
        bool Delivering(bool flag);
        bool IsDelivering();
        Message Receive();
        void Send(Message message);
    }
}