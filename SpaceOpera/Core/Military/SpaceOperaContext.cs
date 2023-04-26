using Cardamom.Graphing.BehaviorTree;

namespace SpaceOpera.Core.Military
{
    public class SpaceOperaContext : SimpleContext
    {
        public class FormationContext : SpaceOperaContext
        {
            public FormationDriver Driver { get; }

            internal FormationContext(World world, FormationDriver driver)
                : base(world)
            {
                Driver = driver;
            }
        }

        public World World { get; }

        public SpaceOperaContext(World world)
        {
            World = world;
        }

        public FormationContext ForFleet(FormationDriver fleet)
        {
            return new FormationContext(World, fleet);
        }
    }
}
