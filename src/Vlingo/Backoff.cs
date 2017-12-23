using System.Threading;

namespace Vlingo
{
    public sealed class Backoff
    {
        private const int BackoffCap = 4096;
        private const int BackoffReset = 0;
        private const int BackoffStart = 1;
        private readonly bool _fixed;

        private int _backoff;

        public Backoff()
        {
            _backoff = BackoffReset;
            _fixed = false;
        }

        public Backoff(int fixedBackoff)
        {
            _backoff = fixedBackoff;
            _fixed = true;
        }

        public void Now()
        {
            if (!_fixed)
            {
                if (_backoff == BackoffReset)
                {
                    _backoff = BackoffStart;
                }
                else if (_backoff < BackoffCap)
                {
                    _backoff = _backoff * 2;
                }
            }
            YieldFor(_backoff);
        }

        public void Reset()
        {
            _backoff = BackoffReset;
        }

        private static void YieldFor(int aMillis)
        {
            try
            {
                Thread.Sleep(aMillis);
            }
            catch (ThreadInterruptedException ex)
            {
                // ignore
            }
        }
    }
}