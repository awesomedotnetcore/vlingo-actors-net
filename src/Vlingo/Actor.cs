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

        internal Environment Environment => _environment;

        protected Definition Definition
        {
            get
            {
                if (_environment.IsSecured())
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
                if (_environment.IsSecured())
                {
                    throw new InvalidOperationException("A secured actor cannot provide its stage.");
                }
                return _environment.Stage;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            return _environment.Address.Equals(((Actor)obj)._environment.Address);
        }

        public override int GetHashCode()
        {
            return _environment.Address.GetHashCode();
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

        public bool IsStopped()
        {
            return _environment.IsStopped();
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


        internal T SelfAs<T>()
        {
            return ActorProxy.CreateFor<T>(this, _environment.Mailbox);
        }

        /// <summary>
        /// internal
        /// </summary>

        protected Actor()
        {
            var env = ActorFactory.ThreadLocalEnvironment.Value;
            _environment = env ?? new Environment();
            ActorFactory.ThreadLocalEnvironment.Value = null;
            _flags = FlagReset;
            InternalSendBeforeStart();
        }

        internal void InternalStop()
        {
            _environment.StopChildren();

            _environment.SetStopped();

            InternalAfterStop();
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

    }
}