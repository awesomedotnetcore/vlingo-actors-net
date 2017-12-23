using System;
using Vlingo.Testkit;

namespace Vlingo
{
    public abstract class Actor : IStoppable
    {
        private const byte FlagReset = 0x00;
        private const byte FlagStopped = 0x01;
        private const byte FlagSecured = 0x02;
        private readonly Environment _environment;
        private byte _flags;
        public Address Address => _environment.Address;

        protected Definition Definition
        {
            get
            {
                if (InternalIsSecured())
                {
                    throw new Exception("A secured actor cannot provide its definition.");
                }
                return _environment.Definition;
            }
        }


        internal Stage Stage
        {
            get
            {
                if (InternalIsSecured())
                {
                    throw new InvalidOperationException("A secured actor cannot provide its stage.");
                }
                return _environment.Stage;
            }
        }

        protected Actor()
        {
            _environment = ActorFactory.ThreadLocalEnvironment.Value;
            ActorFactory.ThreadLocalEnvironment.Value = null;
            _flags = FlagReset;
            InternalSendBeforeStart();
        }

        public bool IsStopped()
        {
            return InternalIsStopped();
        }

        public void Stop()
        {
            if (!IsStopped())
            {
                if (_environment.Address.Id != World.DeadlettersId)
                {
                    _environment.Stage.Stop(this);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            return _environment.Address.Equals(((Actor) obj)._environment.Address);
        }

        public override int GetHashCode()
        {
            return _environment.Address.GetHashCode();
        }

        public void Secure()
        {
            _flags |= FlagSecured;
        }

        public override string ToString()
        {
            return $"Actor[type={GetType().Name} address={_environment.Address}]";
        }


        public TestState ViewTestState()
        {
            // override for concrete actor state
            return new TestState();
        }

        protected void AfterRestart(Exception reason)
        {
            // override
            BeforeStart();
        }

        protected virtual void AfterStop()
        {
            // override
        }

        protected void BeforeRestart(Exception reason)
        {
            // override
            AfterStop();
        }

        protected void BeforeStart()
        {
            // override
        }

        protected T ChildActorFor<T>(Definition definition)
        {
            return _environment.Stage.ActorFor<T>(definition, this);
        }

        protected Actor Parent()
        {
            if (InternalIsSecured())
            {
                throw new InvalidOperationException("A secured actor cannot provide its parent.");
            }
            return InternalParent();
        }

        protected T SelfAs<T>()
        {
            return ActorProxy.CreateFor<T>(this, _environment.Mailbox);
        }

        protected IOutcomeInterest<object> SelfAsOutcomeInterest(object reference)
        {
            var outcomeAware = ActorProxy.CreateFor<IOutcomeAware<object, object>>(typeof(IOutcomeAware<object, object>), this, _environment.Mailbox);
            return new OutcomeInterestActorProxy<object, object>(outcomeAware, reference);
        }

        protected Stage StageNamed(string name)
        {
            return _environment.Stage.World.StageNamed(name);
        }

        internal void InternalAddChild(Actor child)
        {
            _environment.Children.Add(child);
        }

        internal void InternalAfterStop()
        {
            try
            {
                AfterStop();
            }
            catch (Exception ex)
            {
                // TODO: Log
                // TODO: Supervise
                Console.WriteLine($"vlingo/actors: Actor afterStop() failed: {ex.Message}");
            }
        }


        internal void InternalBeforeStart()
        {
            try
            {
                BeforeStart();
            }
            catch (Exception ex)
            {
                // TODO: Log
                // TODO: Supervise
                Console.WriteLine($"vlingo/actors: Actor beforeStart() failed: {ex.Message}");
            }
        }

        internal Environment InternalEnvironment()
        {
            return _environment;
        }


        internal bool InternalIsSecured()
        {
            return (_flags & FlagSecured) == FlagSecured;
        }

        internal bool InternalIsStopped()
        {
            return (_flags & FlagStopped) == FlagStopped;
        }

        internal Actor InternalParent()
        {
            return _environment.Parent;
        }

        internal void InternalSendBeforeStart()
        {
            try
            {
                var method = typeof(Actor).GetMethod(nameof(InternalBeforeStart));
                var message = new Message(this, method, new object[] { });
                _environment.Mailbox.Send(message);
            }
            catch (Exception ex)
            {
                InternalBeforeStart();
            }
        }


        internal void InternalSetStopped()
        {
            _flags |= FlagStopped;
        }


        internal void InternalStop()
        {
            InternalStopChildren();

            InternalSetStopped();

            InternalAfterStop();
        }

        internal void InternalStopChildren()
        {
            for (var i = 0; i < _environment.Children.Count;)
            {
                var actor = _environment.Children[i];
                actor.SelfAs<IStoppable>().Stop();
                _environment.Children.RemoveAt(i);
            }
        }
    }
}