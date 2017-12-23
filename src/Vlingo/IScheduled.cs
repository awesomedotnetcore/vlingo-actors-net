namespace Vlingo
{
    public interface IScheduled
    {
        void IntervalSignal(IScheduled scheduled, object data);
    }
}