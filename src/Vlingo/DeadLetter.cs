using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vlingo
{
    public sealed class DeadLetter
    {
        public Actor Actor { get; }
        public object[] Args { get; }
        public string MethodName { get; }

        internal DeadLetter(Actor actor, MethodInfo method, object[] args)
        {
            Actor = actor;
            MethodName = method.Name;
            Args = args;
        }


        public override string ToString()
        {
            return $"DeadLetter[{Actor}#{MethodName}{ArgsToInvocation(Args)}]";
        }

        private static string ArgsToInvocation(IReadOnlyList<object> args)
        {
            if (args == null)
            {
                return "()";
            }

            var builder = new StringBuilder("(");
            var max = Math.Min(10, args.Count);

            for (var idx = 0; idx < max; ++idx)
            {
                if (idx > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(args[idx]);
            }

            if (max < args.Count)
            {
                builder.Append(", ...");
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}