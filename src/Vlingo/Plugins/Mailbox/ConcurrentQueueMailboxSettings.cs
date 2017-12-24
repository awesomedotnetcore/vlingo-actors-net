namespace Vlingo.Plugins.Mailbox
{
    public class ConcurrentQueueMailboxSettings
    {
        private static ConcurrentQueueMailboxSettings _settings;

        public int ThrottlingCount { get; private set; }

        protected static ConcurrentQueueMailboxSettings Instance()
        {
            return _settings;
        }

        protected static void With(int throttlingCount)
        {
            _settings = new ConcurrentQueueMailboxSettings(throttlingCount);
        }

        private ConcurrentQueueMailboxSettings(int throttlingCount)
        {
            ThrottlingCount = throttlingCount;
        }
    }
}