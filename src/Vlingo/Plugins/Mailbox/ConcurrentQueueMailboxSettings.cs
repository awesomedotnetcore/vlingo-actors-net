namespace Vlingo.Plugins.Mailbox
{
    public class ConcurrentQueueMailboxSettings
    {
        private static ConcurrentQueueMailboxSettings _settings;

        public int ThrottlingCount { get; }

        private ConcurrentQueueMailboxSettings(int throttlingCount)
        {
            ThrottlingCount = throttlingCount;
        }

        public static ConcurrentQueueMailboxSettings Instance => _settings;

        public static void With(int throttlingCount)
        {
            _settings = new ConcurrentQueueMailboxSettings(throttlingCount);
        }
    }
}