using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Vlingo.Plugins.Mailbox
{
    public class ExecutorDispatcher : IDispatcher
    {
        private volatile bool _closed;
        private readonly ActionBlock<IMailbox> _actionBlock;

        public ExecutorDispatcher(int availableThreads, float numberOfDispatchersFactor,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var numberOfThreads = (int)(availableThreads * numberOfDispatchersFactor);

            _actionBlock = new ActionBlock<IMailbox>(box => box.Run(), new ExecutionDataflowBlockOptions()
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = numberOfThreads,
            });
        }

        public void Close()
        {
            _closed = true;
            _actionBlock.Complete();
            _actionBlock.Completion.Wait();
        }

        public void Execute(IMailbox mailbox)
        {
            if (_closed)
            {
                return;
            }

            if (mailbox.Delivering(true))
            {
                _actionBlock.Post(mailbox);
            }
        }

        public bool RequiresExecutionNotification()
        {
            return false;
        }
    }
}