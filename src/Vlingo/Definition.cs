using System;
using System.Collections.Generic;
using System.Linq;

namespace Vlingo
{
    public sealed class Definition
    {
        public static readonly IEnumerable<object> NoParameters = Enumerable.Empty<object>();
        public string ActorName { get; }

        public string MailboxName { get; }
        internal IEnumerable<object> Parameters { get; }

        public Actor Parent { get; }
        public Type Type { get; }


        public Definition(Type type, IEnumerable<object> parameters, Actor parent, string actorName)
            : this(type, parameters, parent, null, actorName)
        {
        }

        public Definition(Type type, IEnumerable<object> parameters, string actorName)
            : this(type, parameters, null, null, actorName)
        {
        }


        public Definition(Type type, IEnumerable<object> parameters, Actor parent, string mailboxName, string actorName)
        {
            Type = type;
            Parameters = parameters;
            Parent = parent;
            MailboxName = mailboxName;
            ActorName = actorName;
        }

        public static Definition Has<TActor>(IEnumerable<object> parameters, string actorName) where TActor : Actor
        {
            var type = typeof(TActor);
            return new Definition(type, parameters, actorName);
        }

        public static Definition Has<TActor>(IEnumerable<object> parameters, Actor parent, string actorName) where TActor : Actor
        {
            var type = typeof(TActor);
            return new Definition(type, parameters, parent, actorName);
        }


        public Actor ParentOr(Actor defaultParent)
        {
            return Parent ?? defaultParent;
        }
    }
}