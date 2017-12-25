namespace Vlingo
{
    public interface IMailbox 
    {
        void Close();
        bool Delivering(bool flag);
        bool IsDelivering();
        Message Receive();
        void Run();
        void Send(Message message);
    }
}