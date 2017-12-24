using System;
using System.Collections.Concurrent;
using System.Linq;
using Vlingo.Infra;

namespace Vlingo.Plugins.Mailbox
{
    public class ConcurrentQueueMailbox
        : IMailbox
    {
        private readonly InterlockedBoolean _delivering = new InterlockedBoolean();
        private readonly IDispatcher _dispatcher;
        private readonly ConcurrentQueue<Message> _queue = new ConcurrentQueue<Message>();

        public ConcurrentQueueMailbox(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Run()
        {
            var total = ConcurrentQueueMailboxSettings.Instance.ThrottlingCount;
            for (var count = 0; count < total; ++count)
            {
                var message = Receive();
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
            _queue.TryDequeue(out var message);
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