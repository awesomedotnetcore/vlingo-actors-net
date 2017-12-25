using System;
using System.Threading;

namespace Vlingo.Plugins.Mailbox
{
    public class ConcurrentQueueMailboxPlugin
        : IPlugin, IMailboxProvider
    {
        private IDispatcher _executorDispatcher;

        public IMailbox ProvideMailboxFor(int hashCode)
        {
            return new ConcurrentQueueMailbox(_executorDispatcher);
        }

        public IMailbox ProvideMailboxFor(int hashCode, IDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            return new ConcurrentQueueMailbox(dispatcher);
        }

        void IMailboxProvider.Close()
        {
            _executorDispatcher.Close();
        }

        public string Name { get; private set; }

        void IPlugin.Close()
        {
            _executorDispatcher.Close();
        }


        public void Start(IRegistrar registrar, string name, PluginProperties properties, CancellationToken cancellationToken = default(CancellationToken))
        {
            Name = name;
            ConcurrentQueueMailboxSettings.With(properties.GetInteger("dispatcherThrottlingCount", 1));

            CreateExecutorDispatcher(properties, cancellationToken);

            RegisterWith(registrar, properties);
        }

        private void CreateExecutorDispatcher(PluginProperties properties, CancellationToken cancellationToken)
        {
            var numberOfDispatchersFactor = properties.GetFloat("numberOfDispatchersFactor", 1.5f);

            _executorDispatcher =
                new ExecutorDispatcher(
                    System.Environment.ProcessorCount,
                    numberOfDispatchersFactor, cancellationToken);
        }

        private void RegisterWith(IRegistrar registrar, PluginProperties properties)
        {
            var defaultMailbox = properties.GetBoolean("defaultMailbox", true);

            registrar.Register(Name, defaultMailbox, this);
        }
    }
}