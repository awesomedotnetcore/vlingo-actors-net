using System;

namespace Vlingo
{
    public class ActorProxy
    {
        public static T CreateFor<T>(Actor actor, IMailbox mailbox)
        {
            return default(T);
        }

        public static T CreateFor<T>(Type type, Actor actor, IMailbox mailbox)
        {
            return default(T);
        }
    }
}