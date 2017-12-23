using System;
using System.Threading;

namespace Vlingo
{
    public class Stage : IStoppable
    {
        private readonly Directory _directory;
        private bool _stopped;
        public int Count => _directory.Count;
        private string Name { get; }
        public World World { get; }

        internal Stage(World world, string name)
        {
            World = world;
            Name = name;
            _directory = new Directory();
            _stopped = false;
        }

        public bool IsStopped()
        {
            return _stopped;
        }

        public void Stop()
        {
            Sweep();

            // TODO: remove...
            Dump();
            ;
            var retries = 0;
            while (Count > 1 && ++retries < 10)
            {
                try
                {
                    Thread.Sleep(10);
                }
                catch (Exception e)
                {
                }
            }

            _stopped = true;
        }


        public T ActorFor<T>(Definition definition, Actor parent)
        {
            return ActorFor<T>(definition, parent, null, null).ProtocolActor;
        }

        public object ActorFor(Definition definition, Type[] protocols, Actor parent)
        {
            return ActorFor(definition, protocols, parent, null, null).ProtocolActor;
        }


        public T ActorFor<T>(Definition definition)
        {
            return ActorFor<T>(definition, definition.ParentOr(World.DefaultParent));
        }

        public object ActorFor(Definition definition, Type[] protocols)
        {
            return ActorFor(definition, protocols, definition.ParentOr(World.DefaultParent));
        }

        public void Dump()
        {
            Console.WriteLine($"STAGE: {Name}");
            _directory.Dump();
        }


        public void Stop(Actor actor)
        {
            var removedActor = _directory.Remove(actor.Address);

            if (actor == removedActor)
            {
                removedActor.InternalStop();
            }
        }

        internal ActorProtocolActor<T> ActorFor<T>(Definition definition, Actor parent, Address maybeAddress, IMailbox maybeMailbox)
        {
            var protocol = typeof(T);
            try
            {
                //Actor actor = CreateRawActor(definition, parent, maybeAddress, maybeMailbox);
                //T protocolActor = ActorProxy.CreateFor<T>(protocol, actor, actor.InternalOnlyEnvironment().Mailbox);
                //return new ActorProtocolActor<T>(actor, protocolActor);
                return null;
            }
            catch (Exception e)
            {
                // TODO: deal with this
                Console.WriteLine($"vlingo/actors: FAILED: {e.Message}");

                return null;
            }
        }

        private ActorProtocolActor<object> ActorFor(Definition definition, Type[] protocols, Actor parent, Address maybeAddress, IMailbox maybeMailbox)
        {
            try
            {
                // Actor actor = createRawActor(definition, parent, maybeAddress, maybeMailbox);
                // Object protocolActor = ActorProxy.createFor(protocols, actor, actor.__internalOnlyEnvironment().mailbox);
                //return new ActorProtocolActor<Object>(actor, protocolActor);
                return null;
            }
            catch (Exception e)
            {
                // TODO: deal with this
                Console.WriteLine($"vlingo/actors: FAILED: {e.Message}");
                return null;
            }
        }

        private Actor CreateRawActor(Definition definition, Actor parent, Address maybeAddress, IMailbox maybeMailbox)
        {
            if (IsStopped())
            {
                throw new Exception("Actor stage has been stopped.");
            }

            var address = maybeAddress ?? Address.From(definition.ActorName);

            if (_directory.IsRegistered(address))
            {
                throw new Exception("Address already exists: " + address);
            }

            var mailbox = maybeMailbox ?? ActorFactory.ActorMailbox(this, address, definition);

            var actor = ActorFactory.ActorFor(this, parent, definition, address, mailbox);

            _directory.Register(actor.Address, actor);

            actor.InternalBeforeStart();

            return actor;
        }

        private void Sweep()
        {
            World.PrivateRoot?.Stop();
        }
    }

    internal class ActorProtocolActor<T>
    {
        public Actor Actor { get; }
        public T ProtocolActor { get; }

        protected ActorProtocolActor(Actor actor, T protocol)
        {
            Actor = actor;
            ProtocolActor = protocol;
        }

        // todo 
        //protected TestActor<T> toTestActor()
        //{
        //    return new TestActor<T>(actor, protocolActor, actor.address());
        //}
    }
}