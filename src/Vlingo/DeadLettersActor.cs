using System;
using System.Collections.Generic;

namespace Vlingo
{
    public class DeadLettersActor
        : Actor, IDeadLetters
    {
        private readonly List<IDeadLettersListener> _listeners;

        public DeadLettersActor()
        {
            _listeners = new List<IDeadLettersListener>();

            Stage.World.SetDeadLetters(SelfAs<IDeadLetters>());
        }

        public void FailedDelivery(DeadLetter deadLetter)
        {
            // TODO: Use logging
            Console.WriteLine($"vlingo/actors: {deadLetter}");

            foreach (var listener in _listeners)
            {
                try
                {
                    listener.Handle(deadLetter);
                }
                catch (Exception e)
                {
                    // ignore (log?)
                }
            }
        }

        public void RegisterListener(IDeadLettersListener listener)
        {
            _listeners.Add(listener);
        }

        protected override void AfterStop()
        {
            Stage.World.SetDeadLetters(null);
            base.AfterStop();
        }
    }
}