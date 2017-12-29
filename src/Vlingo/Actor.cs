using System;
using Vlingo.Testkit;

namespace Vlingo
{
    public abstract class Actor : IStoppable
    {
        public Address Address => Environment.Address;

        internal Environment Environment { get; }

        internal Definition Definition
        {
            get
            {
                if (Environment.IsSecured())
                {
                    throw new Exception("A secured actor cannot provide its definition.");
                }
                return Environment.Definition;
            }
        }


        internal Stage Stage
        {
            get
            {
                if (Environment.IsSecured())
                {
                    throw new InvalidOperationException("A secured actor cannot provide its stage.");
                }
                return Environment.Stage;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            return Environment.Address.Equals(((Actor)obj).Environment.Address);
        }

        public override int GetHashCode()
        {
            return Environment.Address.GetHashCode();
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
            return Environment.IsStopped();
        }

        public void Stop()
        {
            if (!IsStopped())
            {
                if (Environment.Address.Id != World.DeadlettersId)
                {
                    Environment.Stage.Stop(this);
                }
            }
        }


        internal T SelfAs<T>()
        {
            return ActorProxy.CreateFor<T>(this, Environment.Mailbox);
        }

        /// <summary>
        /// internal
        /// </summary>

        protected Actor()
        {
            var env = ActorFactory.ThreadLocalEnvironment.Value;
            Environment = env ?? new Environment();
            ActorFactory.ThreadLocalEnvironment.Value = null;
            InternalSendBeforeStart();
        }

        internal void InternalStop()
        {
            Environment.StopChildren();

            Environment.SetStopped();

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
                Environment.Mailbox.Send(message);
            }
            catch (Exception ex)
            {
                InternalBeforeStart();
            }
        }

    }
}