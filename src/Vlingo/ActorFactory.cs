using System;
using System.Linq;
using System.Threading;

namespace Vlingo
{
    public class ActorFactory
    {
        internal static readonly ThreadLocal<Environment> ThreadLocalEnvironment = new ThreadLocal<Environment>();

        public static Actor ActorFor(Stage stage, Actor parent, Definition definition, Address address, IMailbox mailbox)
        {
            var environment = new Environment(
                stage,
                address,
                definition,
                parent,
                mailbox);

            ThreadLocalEnvironment.Value = environment;

            Actor actor = null;

            if (!definition.Parameters.Any())
            {
                actor = Activator.CreateInstance(definition.Type) as Actor;
            }
            else
            {
                // currently supports only one constructor
                actor = Activator.CreateInstance(definition.Type, definition.Parameters) as Actor;
                ;
            }

            parent?.InternalAddChild(actor);

            return actor;
        }

        public static IMailbox ActorMailbox(Stage stage, Address address, Definition definition)
        {
            var mailboxName = stage.World.MailboxNameFrom(definition.MailboxName);
            var mailbox = stage.World.AssignMailbox(mailboxName, address.GetHashCode());

            return mailbox;
        }
    }
}