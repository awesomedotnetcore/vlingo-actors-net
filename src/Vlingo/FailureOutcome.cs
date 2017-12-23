namespace Vlingo
{
    public class FailureOutcome<TO>
        : Outcome<TO>
    {
        public FailureOutcome(TO value) : base(value)
        {
        }
    }
}