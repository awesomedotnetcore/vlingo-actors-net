namespace Vlingo
{
    public class SuccessfulOutcome<TO> : Outcome<TO>
    {
        public SuccessfulOutcome(TO value) : base(value)
        {
        }
    }
}