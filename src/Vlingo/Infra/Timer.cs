using System;
using System.Threading;

/*
 * 
 * Name    : Java like Timer in C# 
 * Url     : https://timerutil.codeplex.com/SourceControl/latest#TimerUtil/TimerUtil/Timer.cs
 * License : Apache License 2.0 (Apache)
 * 
 */

namespace Vlingo.Infra
{
    /// <summary>
    ///     C# equivalent of java.util.Timer.schedule()
    /// </summary>
    public sealed class Timer : IDisposable
    {
        /// <summary>
        ///     Number of milliseconds in a day = 86400000
        /// </summary>
        public const int MillisecondsInADay = 1000 * 60 * 60 * 24;

        /// <summary>
        ///     Number of milliseconds in an hour = 3600000
        /// </summary>
        public const int MillisecondsInAnHour = 1000 * 60 * 60;

        /// <summary>
        ///     Number of milliseconds in a minute = 60000
        /// </summary>
        public const int MillisecondsInAMinute = 1000 * 60;

        private readonly System.Timers.Timer _systemTimer;

        /// <summary>
        ///     Creates a new timer.
        /// </summary>
        public Timer()
        {
            _systemTimer = new System.Timers.Timer();
        }

        public void Dispose()
        {
            _systemTimer.Dispose();
        }

        /// <summary>
        ///     Terminates this timer, discarding any currently scheduled tasks. Does not interfere with a currently executing task
        ///     (if it exists).
        ///     Once a timer has been terminated, its execution thread terminates gracefully, and no more tasks may be scheduled on
        ///     it.
        ///     Note that calling this method from within the run method of a timer task that was invoked by this timer absolutely
        ///     guarantees that the ongoing task execution is the last task execution that will ever be performed by this timer.
        /// </summary>
        public void Cancel()
        {
            _systemTimer.Stop();
        }

        //
        // Summary:
        //     Returns a DateTime object of future with the speified hour, minute, second and millisecond
        //
        // Parameters:
        //   hour:
        //     The hours (0 through 23).
        //
        //   minute:
        //     The minutes (0 through 59).
        //
        //   second:
        //     The seconds (0 through 59).
        //
        //   millisecond:
        //     The milliseconds (0 through 999).
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     hour is less than 0 or greater than 23.-or- minute is less than 0 or greater
        //     than 59.-or- second is less than 0 or greater than 59.-or- millisecond is
        //     less than 0 or greater than 999.
        public static DateTime GetFutureTime(int hour = 0, int minute = 0, int second = 0, int millisecond = 0)
        {
            var now = DateTime.Now;
            var schedule = new DateTime(now.Year, now.Month, now.Day, hour, minute, second, millisecond);
            if (schedule <= now)
            {
                schedule.AddDays(1);
            }
            return schedule;
        }

        /// <summary>
        ///     Schedules the specified task for execution after the specified delay.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="delay"></param>
        /// <param name="start"></param>
        public void Schedule(ParameterizedThreadStart start, object parameter, long delay)
        {
            _systemTimer.AutoReset = false;
            _systemTimer.Interval = delay;
            _systemTimer.Elapsed += (sender, e) => { new Thread(start).Start(parameter); };
            _systemTimer.Start();
        }

        public void Schedule(ThreadStart start, long delay)
        {
            _systemTimer.AutoReset = false;
            _systemTimer.Interval = delay;
            _systemTimer.Elapsed += (sender, e) => { new Thread(start).Start(); };
            _systemTimer.Start();
        }

        /// <summary>
        ///     Schedules the specified task for execution at the specified time.
        ///     If the time is in the past, the task is scheduled for immediate execution.
        /// </summary>
        /// <param name="start">Task to be scheduled</param>
        /// <param name="parameter">An object that contains data for the thread procedure</param>
        /// <param name="time">Time at which task is to be executed</param>
        public void Schedule(ParameterizedThreadStart start, object parameter, DateTime time)
        {
            Schedule(start, parameter, Math.Max(1, Convert.ToInt64((time - DateTime.Now).TotalMilliseconds)));
        }

        /// <summary>
        ///     Schedules the specified task for execution at the specified time.
        ///     If the time is in the past, the task is scheduled for immediate execution.
        /// </summary>
        /// <param name="start">Task to be scheduled</param>
        /// <param name="time">Time at which task is to be executed</param>
        public void Schedule(ThreadStart start, DateTime time)
        {
            Schedule(start, Math.Max(1, Convert.ToInt64((time - DateTime.Now).TotalMilliseconds)));
        }

        /// <summary>
        ///     Schedules the specified task for repeated fixed-delay execution, beginning after the specified delay.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="delay"></param>
        /// <param name="period"></param>
        /// <param name="start"></param>
        public void Schedule(ParameterizedThreadStart start, object parameter, long delay, long period)
        {
            Schedule(() =>
            {
                // start time is over. Execute the task very period.
                new Thread(start).Start(parameter);
                if (_systemTimer.AutoReset != true)
                {
                    _systemTimer.AutoReset = true;
                }
                if (_systemTimer.Interval != period)
                {
                    _systemTimer.Interval = period;
                }
            }, delay);
        }

        public void Schedule(ThreadStart start, long delay, long period)
        {
            Schedule(() =>
            {
                // start time is over. Execute the task very period.
                new Thread(start).Start();
                if (_systemTimer.AutoReset != true)
                {
                    _systemTimer.AutoReset = true;
                }
                if (_systemTimer.Interval != period)
                {
                    _systemTimer.Interval = period;
                }
            }, delay);
        }

        /// <summary>
        ///     Schedules the specified task for repeated fixed-delay execution, beginning at the specified time.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="firstTime"></param>
        /// <param name="period"></param>
        /// <param name="start"></param>
        public void Schedule(ParameterizedThreadStart start, object parameter, DateTime firstTime, long period)
        {
            Schedule(() =>
            {
                // start time is over. Execute the task very period.
                new Thread(start).Start(parameter);
                if (_systemTimer.AutoReset != true)
                {
                    _systemTimer.AutoReset = true;
                }
                if (_systemTimer.Interval != period)
                {
                    _systemTimer.Interval = period;
                }
            }, firstTime);
        }

        public void Schedule(ThreadStart start, DateTime firstTime, long period)
        {
            Schedule(() =>
            {
                // start time is over. Execute the task very period.
                new Thread(start).Start();
                if (_systemTimer.AutoReset != true)
                {
                    _systemTimer.AutoReset = true;
                }
                if (_systemTimer.Interval != period)
                {
                    _systemTimer.Interval = period;
                }
            }, firstTime);
        }
    }
}