using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Orders
{
    public class CreateDesignOrder : IOrder
    {
        public Faction Faction { get; }
        public DesignConfiguration Design { get; }

        public CreateDesignOrder(Faction faction, DesignConfiguration design)
        {
            Faction = faction;
            Design = design;
        }

        public ValidationFailureReason Validate(World world)
        {
            return Design.Validate() ? ValidationFailureReason.None : ValidationFailureReason.InvalidDesign;
        }

        public bool Execute(World World)
        {
            var design = World.DesignBuilder.Build(Design);
            World.AddDesign(design);
            World.AddLicense(new DesignLicense(Faction, design));
            return true;
        }
    }
}