using Cardamom.Graphing.BehaviorTree;

namespace SpaceOpera.Core.Military.Ai
{
    public class SpaceOperaContext : SimpleContext
    {
        public class FormationContext : SpaceOperaContext
        {
            public IAtomicFormation Formation { get; }

            internal FormationContext(World world, IAtomicFormation formation)
                : base(world)
            {
                Formation = formation;
            }
        }

        public World World { get; }

        public SpaceOperaContext(World world)
        {
            World = world;
        }

        public FormationContext ForFormation(IAtomicFormation formation)
        {
            return new FormationContext(World, formation);
        }
    }
}
