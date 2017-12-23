namespace Vlingo
{
    public abstract class Outcome<TO>
    {
        public TO Value { get; }

        protected Outcome(TO value)
        {
            Value = value;
        }
    }
}