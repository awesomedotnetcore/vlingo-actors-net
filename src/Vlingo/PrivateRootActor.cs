namespace Vlingo
{
    public class PrivateRootActor
        : Actor
    {
        public PrivateRootActor()
        {
            Stage.World.SetPrivateRoot(SelfAs<IStoppable>());

            Stage.ActorFor<INoProtocol>(
                Definition.Has<PublicRootActor>(Definition.NoParameters, World.PublicRootName),
                this,
                new Address(World.PublicRootId, World.PublicRootName),
                null);

            Stage.ActorFor<IDeadLetters>(
                Definition.Has<DeadLettersActor>(Definition.NoParameters, World.DeadlettersName),
                this,
                new Address(World.DeadlettersId, World.DeadlettersName),
                null);
        }

        protected override void AfterStop()
        {
            Stage.World.SetPrivateRoot(null);
            base.AfterStop();
        }
    }
}