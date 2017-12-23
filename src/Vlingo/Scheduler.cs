using System;
using Vlingo.Infra;

namespace Vlingo
{
    public class Scheduler
    {
        private readonly Timer _timer;

        public Scheduler()
        {
            _timer = new Timer();
        }

        public ICancellable Schedule(IScheduled scheduled, object data, long delayBefore, long interval)
        {
            var schedulerTask = new SchedulerTask(scheduled, data, true);
            _timer.Schedule(schedulerTask, delayBefore, interval);
            return schedulerTask;
        }

        public ICancellable ScheduleOnce(IScheduled scheduled, object data, long delayBefore, long interval)
        {
            var schedulerTask = new SchedulerTask(scheduled, data, false);
            _timer.Schedule(schedulerTask, delayBefore, interval);
            return schedulerTask;
        }

        internal void Close()
        {
            _timer.Cancel();
        }

        private class SchedulerTask : ITimerTask, ICancellable
        {
            private readonly object _data;
            private readonly bool _repeats;
            private readonly IScheduled _scheduled;

            internal SchedulerTask(IScheduled scheduled, object data, bool repeats)
            {
                _scheduled = scheduled;
                _data = data;
                _repeats = repeats;
            }

            public bool Cancel()
            {
                // todo : need implement cancel
                throw new NotImplementedException();
            }


            public void Run()
            {
                _scheduled.IntervalSignal(_scheduled, _data);

                if (!_repeats)
                {
                    Cancel();
                }
            }
        }
    }
}