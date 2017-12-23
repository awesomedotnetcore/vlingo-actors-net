namespace Vlingo
{
    public interface IOutcomeInterest<TO>
    {
        void FailureOutcome(Outcome<TO> outcome);
        void SuccessfulOutcome(Outcome<TO> outcome);
    }
}