namespace Vlingo
{
    public interface IOutcomeAware<TO, in TR>
    {
        void FailureOutcome(Outcome<TO> outcome, TR reference);
        void SuccessfulOutcome(Outcome<TO> outcome, TR reference);
    }
}