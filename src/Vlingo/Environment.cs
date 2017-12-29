using System.Collections.Generic;

namespace Vlingo
{
    internal class Environment
    {
        private const byte FlagReset = 0x00;
        private const byte FlagStopped = 0x01;
        private const byte FlagSecured = 0x02;


        private byte _flags;

        public Address Address { get; }

        public List<Actor> Children { get; }

        public Definition Definition { get; }

        public IMailbox Mailbox { get; }

        public Actor Parent { get; }

        public Stage Stage { get; }

        internal Environment(Stage stage, Address address, Definition definition, Actor parent, IMailbox mailbox)
        {
            //assert((this.stage != null));
            Stage = stage;
            // assert((this.address != null));
            Address = address;
            //assert((this.definition != null));
            Definition = definition;
            if (Address.Id != World.PrivateRootId)
            {
                // assert((this.parent != null));
            }

            Parent = parent;
            // assert((this.mailbox != null));
            Mailbox = mailbox;
            Children = new List<Actor>(2);

            _flags = FlagReset;
        }

        public Environment()
        {
            // for testing
            Address = Address.From("test");
            Children = new List<Actor>();
            Definition = Definition.Has(null, Definition.NoParameters);
            _flags = FlagReset;
            Mailbox = null;
            Parent = null;
            Stage = null;
        }

        internal void AddChild(Actor child)
        {
            Children.Add(child);
        }

        internal bool IsSecured()
        {
            return (_flags & FlagSecured) == FlagSecured;
        }

        internal void SetSecured()
        {
            _flags |= FlagSecured;
        }

        internal bool IsStopped()
        {
            return (_flags & FlagStopped) == FlagStopped;
        }

        internal void SetStopped()
        {
            _flags |= FlagStopped;
        }

        internal void StopChildren()
        {
            for (var i = 0; i < Children.Count;)
            {
                var actor = Children[i];
                actor.SelfAs<IStoppable>().Stop();
                Children.RemoveAt(i);
            }
        }
    }
}