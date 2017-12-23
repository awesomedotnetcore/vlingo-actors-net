namespace Vlingo
{
    public interface IDeadLettersListener
    {
        void Handle(DeadLetter deadLetter);
    }
}