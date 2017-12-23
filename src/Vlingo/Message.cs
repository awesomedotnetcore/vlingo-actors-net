using System;
using System.Reflection;

namespace Vlingo
{
    public sealed class Message
    {
        public Actor Actor { get; }
        public object[] Args { get; }
        public MethodInfo MethodInfo { get; }

        public Message(Actor actor, MethodInfo method, object[] args)
        {
            Actor = actor;
            MethodInfo = method;
            Args = args;
        }

        public void Deliver()
        {
            if (Actor.IsStopped())
            {
                var deadLetter = new DeadLetter(Actor, MethodInfo, Args);
                var deadLetters = Actor.Stage.World.DeadLetters;
                if (deadLetters != null)
                {
                    deadLetters.FailedDelivery(deadLetter);
                }
                else
                {
                    // TODO: Log

                    Console.WriteLine("vlingo/actors: MISSING DEAD LETTERS FOR: " + deadLetter);
                }
                return;
            }

            try
            {
                MethodInfo.Invoke(Actor, Args);
            }
            catch (InvalidOperationException e)
            {
                // TODO: handle
                Console.WriteLine($"Message#deliver(): InvalidOperationException: {e.Message} for Actor: {Actor}");
            }
            catch (ArgumentException e)
            {
                // TODO: handle
                Console.WriteLine($"Message#deliver(): ArgumentException: {e.Message} for Actor: {Actor}");
            }
            catch (TargetInvocationException e)
            {
                // TODO: handle
                Console.WriteLine($"Message#deliver(): TargetInvocationException: {e.Message} for Actor: {Actor}");
            }
            catch (Exception e)
            {
                // TODO: handle
                Console.WriteLine($"Message#deliver(): Exception: {e.Message} for Actor: {Actor}");
            }
        }
    }
}