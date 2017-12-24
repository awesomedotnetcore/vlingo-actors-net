using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vlingo.Infra;

namespace Vlingo.Plugins.Mailbox
{
    public class ConcurrentQueueMailbox
        : IMailbox
    {
        private readonly InterlockedBoolean _delivering = new InterlockedBoolean();
        private readonly IDispatcher _dispatcher;
        private readonly ConcurrentQueue<Message> _queue = new ConcurrentQueue<Message>();
        public void Run()
        {
            int total = 0;// ConcurrentQueueMailboxSettings.instance().throttlingCount;
            for (var count = 0; count < total; ++count)
            {
                Message message = Receive();
                if (message != null)
                {
                    message.Deliver();
                }
                else
                {
                    break;
                }
            }
            Delivering(false);
            if (_queue.Any())
            {
                _dispatcher.Execute(this);
            }
        }

        public ConcurrentQueueMailbox(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Close()
        {
            _queue.Clear();
        }

        public bool Delivering(bool flag)
        {
            return _delivering.CompareExchange(!flag, flag);
        }

        public bool IsDelivering()
        {
            return _delivering.Value;
        }

        public Message Receive()
        {
            _queue.TryDequeue(out Message message);
            return message;
        }

        public void Send(Message message)
        {
            _queue.Enqueue(message);
            if (!IsDelivering())
            {
                _dispatcher.Execute(this);
            }
        }
    }
}
