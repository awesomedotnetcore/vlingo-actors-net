using System.Collections.Generic;

namespace Vlingo
{
    internal class Environment
    {
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
        }
    }
}