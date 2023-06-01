using Cardamom.Graphing.BehaviorTree;

namespace SpaceOpera.Core.Military.Ai
{
    public class SpaceOperaContext : SimpleContext
    {
        public class FormationContext : SpaceOperaContext
        {
            public AtomicFormationDriver Driver { get; }

            internal FormationContext(World world, AtomicFormationDriver driver)
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

        public FormationContext ForDriver(AtomicFormationDriver driver)
        {
            return new FormationContext(World, driver);
        }
    }
}
