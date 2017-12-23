namespace Vlingo
{
    public interface IStoppable
    {
        bool IsStopped();
        void Stop();
    }
}