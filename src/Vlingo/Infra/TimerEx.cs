namespace Vlingo.Infra
{
    public static class TimerExtensions
    {
        public static void Schedule(this Timer timer, ITimerTask task, long delayBefore, long interval)
        {
            timer.Schedule(o =>
            {
                if (o is ITimerTask t)
                {
                    t.Run();
                }
            }, task, delayBefore, interval);
        }
    }

    public interface ITimerTask
    {
        void Run();
    }
}