namespace Vlingo.Plugins.Mailbox
{
    public class ExecutorDispatcher : IDispatcher
    {
        private volatile bool _closed;

        /// private readonly ThreadPoolExecutor _executor;
        public ExecutorDispatcher(int availableThreads, float numberOfDispatchersFactor)
        {
            var numberOfThreads = (int) (availableThreads * numberOfDispatchersFactor);
            //this.executor = (ThreadPoolExecutor)Executors.newFixedThreadPool(numberOfThreads);
            //this.executor.setRejectedExecutionHandler(new RejectionHandler());
        }

        //    private class RejectionHandler implements RejectedExecutionHandler
        //    {
        //        protected RejectionHandler() { }

        //    public void rejectedExecution(final Runnable runnable, final ThreadPoolExecutor executor)
        //    {
        //        if (!executor.isShutdown() && !executor.isTerminated())
        //            runnable.run();
        //    }
        //}

        public void Close()
        {
            _closed = true;
            // _executor.Shutdown();
        }

        public void Execute(IMailbox mailbox)
        {
            if (!_closed)
            {
                if (mailbox.Delivering(true))
                {
                    // _executor.execute(mailbox);
                }
            }
        }

        public bool RequiresExecutionNotification()
        {
            return false;
        }
    }
}