using System;
using System.Collections.Generic;
using Vlingo.Plugins;

namespace Vlingo
{
    public sealed class World : IRegistrar
    {
        internal const string PrivateRootName = "#private";
        internal const int PublicRootId = PrivateRootId - 1;
        internal const string PublicRootName = "#public";
        internal const int DeadlettersId = PublicRootId - 1;
        internal const string DeadlettersName = "#deadLetters";
        internal const int PrivateRootId = int.MaxValue;
        private const string DefaultStage = "__defaultStage";

        private static readonly Configuration DefaultConfiguration = new Configuration();

        public IDeadLetters DeadLetters { get; private set; }
        private readonly MailboxProviderKeeper _mailboxProviderKeeper;
        private IStoppable _publicRoot;


        public Dictionary<string, Stage> Stages;
        public Configuration Configuration { get; }
        public Actor DefaultParent { get; private set; }


        public string Name { get; }
        public IStoppable PrivateRoot { get; private set; }
        public Scheduler Scheduler { get; }


        private World(string name, Configuration configuration)
        {
            Name = name;
            Configuration = configuration;
            Scheduler = new Scheduler();
            _mailboxProviderKeeper = new MailboxProviderKeeper();
            Stages = new Dictionary<string, Stage>();

            var defaultStage = new Stage(this, DefaultStage);

            Stages.Add(DefaultStage, defaultStage);

            PluginLoader.LoadPlugins(this);
        }

        public void Register(string name, bool isDefault, IMailboxProvider mailboxProvider)
        {
            _mailboxProviderKeeper.Keep(name, isDefault, mailboxProvider);
        }


        public T ActorFor<T>(Definition definition)
        {
            if (IsTerminated())
            {
                throw new Exception("vlingo/actors: Stopped.");
            }

            return Stage().ActorFor<T>(definition);
        }

        public IMailbox AssignMailbox(string mailboxName, int hashCode)
        {
            return _mailboxProviderKeeper.AssignMailbox(mailboxName, hashCode);
        }

        public bool IsTerminated()
        {
            return Stage().IsStopped();
        }

        public string MailboxNameFrom(string candidateMailboxName)
        {
            if (string.IsNullOrEmpty(candidateMailboxName))
            {
                return FindDefaultMailboxName();
            }
            if (_mailboxProviderKeeper.IsValidMailboxName(candidateMailboxName))
            {
                return candidateMailboxName;
            }
            return FindDefaultMailboxName();
        }

        public void SetDefaultParent(Actor defaultParent)
        {
            if (defaultParent != null && DefaultParent != null)
            {
                throw new Exception("Default parent already exists."); // IllegalStateException
            }

            DefaultParent = defaultParent;
        }

        public void SetPublicRoot(IStoppable publicRoot)
        {
            if (publicRoot != null && _publicRoot != null)
            {
                throw new Exception("The public root already exists."); // IllegalStateException
            }

            _publicRoot = publicRoot;
        }

        public Stage Stage()
        {
            return StageNamed(DefaultStage);
        }

        public Stage StageNamed(string name)
        {
            if (!Stages.TryGetValue(name, out var stage))
            {
                stage = new Stage(this, name);
                Stages[name] = stage;
            }

            return stage;
        }

        public static World Start(string name)
        {
            return Start(name, DefaultConfiguration);
        }

        public static World Start(string name, Configuration configuration)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("The world name must not be null."); // IllegalArgumentException // IllegalArgumentException
            }
            if (configuration == null)
            {
                throw new Exception("The world configuration must not be null."); // IllegalArgumentException
            }

            return new World(name, configuration);
        }

        public void Terminate()
        {
            if (!IsTerminated())
            {
                Scheduler.Close();

                foreach (var stage in Stages)
                {
                    stage.Value.Stop();
                }


                _mailboxProviderKeeper.Close();
            }
        }

        internal string FindDefaultMailboxName()
        {
            return _mailboxProviderKeeper.FindDefault();
        }


        internal void SetDeadLetters(IDeadLetters deadLetters)
        {
            if (deadLetters != null && DeadLetters != null)
            {
                deadLetters.Stop();
                throw new Exception("Dead letters already exists.");
            }

            DeadLetters = deadLetters;
        }

        internal void SetPrivateRoot(IStoppable privateRoot)
        {
            if (privateRoot != null && PrivateRoot != null)
            {
                privateRoot.Stop();
                throw new InvalidOperationException("Private root already exists.");
            }

            PrivateRoot = privateRoot;
        }

        private class MailboxProviderKeeper
        {
            private readonly Dictionary<string, MailboxProviderInfo> _mailboxProviderInfos;

            public MailboxProviderKeeper()
            {
                _mailboxProviderInfos = new Dictionary<string, MailboxProviderInfo>();
            }

           

            public IMailbox AssignMailbox(string mailboxName, int hashCode)
            {
                if (!_mailboxProviderInfos.TryGetValue(mailboxName, out var info))
                {
                    throw new Exception($"No registered MailboxProvider named {mailboxName}");
                    // todo : IllegalStateException
                }

                return info._mailboxProvider.ProvideMailboxFor(hashCode);
            }

            public void Close()
            {
                foreach (var kv in _mailboxProviderInfos)
                {
                    kv.Value._mailboxProvider.Close();
                }
            }

            public string FindDefault()
            {
                foreach (var kv in _mailboxProviderInfos)
                {
                    if (kv.Value._isDefault)
                    {
                        return kv.Value._name;
                    }
                }
                // todo : IllegalStateException
                throw new Exception("No registered default MailboxProvider.");
            }

            public bool IsValidMailboxName(string candidateMailboxName)
            {
                return _mailboxProviderInfos.ContainsKey(candidateMailboxName);
            }

            public void Keep(string name, bool isDefault, IMailboxProvider mailboxProvider)
            {
                if (_mailboxProviderInfos.Count == 0)
                {
                    isDefault = true;
                }

                if (isDefault)
                {
                    UndefaultCurrentDefault();
                }

                _mailboxProviderInfos.Add(name, new MailboxProviderInfo(name, mailboxProvider, isDefault));
            }

            private void UndefaultCurrentDefault()
            {
                foreach (var key in _mailboxProviderInfos.Keys)
                {
                    var info = _mailboxProviderInfos[key];
                    if (info._isDefault)
                    {
                        _mailboxProviderInfos[key] = new MailboxProviderInfo(info._name, info._mailboxProvider, false);
                    }
                }
            }
        }

        private class MailboxProviderInfo
        {
            internal readonly bool _isDefault;
            internal readonly IMailboxProvider _mailboxProvider;
            internal readonly string _name;


            public MailboxProviderInfo(string name, IMailboxProvider mailboxProvider, bool isDefault)
            {
                _name = name;
                _mailboxProvider = mailboxProvider;
                _isDefault = isDefault;
            }
        }
    }
}