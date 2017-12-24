using System;
using System.Collections.Generic;
using System.Text;

namespace Vlingo.Plugins.Mailbox
{
    public class ConcurrentQueueMailboxPlugin
         : IPlugin, IMailboxProvider
    {
        private IDispatcher _executorDispatcher;
        public string Name { get; private set; }

        public ConcurrentQueueMailboxPlugin()
        {

        }

        void IPlugin.Close()
        {
            _executorDispatcher.Close();
        }

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


        public void Start(IRegistrar registrar, string name, PluginProperties properties)
        {
            Name = name;
            //ConcurrentQueueMailboxSettings.with(properties.getInteger("dispatcherThrottlingCount", 1));

            CreateExecutorDispatcher(properties);

            RegisterWith(registrar, properties);
        }

        private void RegisterWith(IRegistrar registrar, PluginProperties properties)
        {
            bool defaultMailbox = false; // properties.getBoolean("defaultMailbox", true);

            registrar.Register(Name, defaultMailbox, this);
        }

        private void CreateExecutorDispatcher(PluginProperties properties)
        {
            float numberOfDispatchersFactor = 1.5f;// properties.getFloat("numberOfDispatchersFactor", 1.5f);

            _executorDispatcher =
                new ExecutorDispatcher(
                   System.Environment.ProcessorCount,
                    numberOfDispatchersFactor);
        }

        void IMailboxProvider.Close()
        {
            _executorDispatcher.Close();
        }
    }
}
