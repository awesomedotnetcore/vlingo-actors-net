using System;
using System.Collections.Generic;
using System.Linq;
using Vlingo.Logging;
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
        private readonly LoggerProviderKeeper _loggerProviderKeeper;
        private readonly MailboxProviderKeeper _mailboxProviderKeeper;
        private ILogger _defaultLogger;
        private IStoppable _publicRoot;


        public Dictionary<string, Stage> Stages;
        public Configuration Configuration { get; }

        public IDeadLetters DeadLetters { get; private set; }
        public Actor DefaultParent { get; private set; }


        public string Name { get; }
        public IStoppable PrivateRoot { get; private set; }
        public Scheduler Scheduler { get; }


        private World(string name, Configuration configuration)
        {
                Name = name;
            Configuration = configuration;
            Scheduler = new Scheduler();
            _loggerProviderKeeper = new LoggerProviderKeeper();
            _mailboxProviderKeeper = new MailboxProviderKeeper();
            Stages = new Dictionary<string, Stage>();

            var defaultStage = new Stage(this, DefaultStage);

            Stages.Add(DefaultStage, defaultStage);

            PluginLoader.LoadPlugins(this);

            defaultStage.ActorFor<IStoppable>(
                Definition.Has<PrivateRootActor>(Definition.NoParameters, PrivateRootName),
                null,
                Address.From(PrivateRootId, PrivateRootName),
                null);
        }

        public void Register(string name, bool isDefault, IMailboxProvider mailboxProvider)
        {
            _mailboxProviderKeeper.Keep(name, isDefault, mailboxProvider);
        }

        public void Register(string name, bool isDefault, ILoggerProvider loggerProvider)
        {
            _loggerProviderKeeper.Keep(name, isDefault, loggerProvider);

            _defaultLogger = _loggerProviderKeeper.FindDefault().CreateLogger();
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


                _loggerProviderKeeper.Close();
                _mailboxProviderKeeper.Close();
            }
        }

        private ILogger FindDefaultLogger()
        {

            ILog l = LogProvider.GetLogger("");
  

            if (_defaultLogger != null)
            {
                return _defaultLogger;
            }

            _defaultLogger = _loggerProviderKeeper.FindDefault().CreateLogger();

            return _defaultLogger;
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

                return info.MailboxProvider.ProvideMailboxFor(hashCode);
            }

            public void Close()
            {
                foreach (var kv in _mailboxProviderInfos)
                {
                    kv.Value.MailboxProvider.Close();
                }
            }

            public string FindDefault()
            {
                foreach (var kv in _mailboxProviderInfos)
                {
                    if (kv.Value.IsDefault)
                    {
                        return kv.Value.Name;
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
                    if (info.IsDefault)
                    {
                        _mailboxProviderInfos[key] = new MailboxProviderInfo(info.Name, info.MailboxProvider, false);
                    }
                }
            }
        }

        private class MailboxProviderInfo
        {
            internal readonly bool IsDefault;
            internal readonly IMailboxProvider MailboxProvider;
            internal readonly string Name;


            public MailboxProviderInfo(string name, IMailboxProvider mailboxProvider, bool isDefault)
            {
                Name = name;
                MailboxProvider = mailboxProvider;
                IsDefault = isDefault;
            }
        }

        private class LoggerProviderInfo
        {
            internal readonly bool IsDefault;
            internal readonly ILoggerProvider LoggerProvider;
            internal readonly string Name;

            internal LoggerProviderInfo(string name, ILoggerProvider loggerProvider, bool isDefault)
            {
                Name = name;
                LoggerProvider = loggerProvider;
                IsDefault = isDefault;
            }
        }

        private class LoggerProviderKeeper
        {
            private readonly Dictionary<string, LoggerProviderInfo> _loggerProviderInfos;

            internal LoggerProviderKeeper()
            {
                _loggerProviderInfos = new Dictionary<string, LoggerProviderInfo>();
            }

            public void Close()
            {
                foreach (var kv in _loggerProviderInfos)
                {
                    kv.Value.LoggerProvider.Close();
                }
            }

            public ILoggerProvider FindDefault()
            {
                var logger = _loggerProviderInfos.Values.FirstOrDefault(f => f.IsDefault);
                if (logger == null)
                {
                    throw new InvalidOperationException("No registered default LoggerProvider.");
                }

                return logger.LoggerProvider;
            }

            public void Keep(string name, bool isDefault, ILoggerProvider loggerProvider)
            {
                if (!_loggerProviderInfos.Any())
                {
                    isDefault = true;
                }

                if (isDefault)
                {
                    UndefaultCurrentDefault();
                }

                _loggerProviderInfos[name] = new LoggerProviderInfo(name, loggerProvider, isDefault);
            }

            private ILoggerProvider FindNamed(string name)
            {
                var logger = _loggerProviderInfos.Values.FirstOrDefault(f => string.Equals(f.Name, name));
                if (logger == null)
                {
                    throw new InvalidOperationException($"No registered LoggerProvider named: {name}");
                }
                return logger.LoggerProvider;
            }

            private void UndefaultCurrentDefault()
            {
                foreach (var kv in _loggerProviderInfos)
                {
                    if (kv.Value.IsDefault)
                    {
                        _loggerProviderInfos[kv.Key] = new LoggerProviderInfo(kv.Value.Name, kv.Value.LoggerProvider, false);
                    }
                }
            }
        }
    }
}