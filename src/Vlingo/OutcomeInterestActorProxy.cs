namespace Vlingo
{
    internal class OutcomeInterestActorProxy<TO, TR> : IOutcomeInterest<TO>
    {
        private readonly IOutcomeAware<TO, TR> _outcomeAware;
        private readonly TR _reference;

        internal OutcomeInterestActorProxy(IOutcomeAware<TO, TR> outcomeAware, TR reference)
        {
            _outcomeAware = outcomeAware;
            _reference = reference;
        }


        public void FailureOutcome(Outcome<TO> outcome)
        {
            _outcomeAware.FailureOutcome(outcome, _reference);
        }


        public void SuccessfulOutcome(Outcome<TO> outcome)
        {
            _outcomeAware.SuccessfulOutcome(outcome, _reference);
        }
    }
}