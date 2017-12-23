namespace Vlingo
{
    public class PublicRootActor
        : Actor
    {
        public PublicRootActor()
        {
            Stage.World.SetDefaultParent(this);
            Stage.World.SetPublicRoot(SelfAs<IStoppable>());
        }

        // TODO: implement top-level supervision
        protected override void AfterStop()
        {
            Stage.World.SetDefaultParent(null);
            Stage.World.SetPublicRoot(null);
            base.AfterStop();
        }
    }
}