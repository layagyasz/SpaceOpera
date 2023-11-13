using Cardamom.Trackers;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics
{
    public class PersistentRoute
    {
        public Faction Faction { get; set; }
        public EconomicSubzoneHolding LeftAnchor { get; }
        public MultiQuantity<IMaterial> LeftMaterials { get; }
        public EconomicSubzoneHolding RightAnchor { get; set; }
        public MultiQuantity<IMaterial> RightMaterials { get; }
        public List<Fleet> AssignedFleets { get; set; }

        public PersistentRoute(
            Faction faction,
            EconomicSubzoneHolding leftAnchor, 
            MultiQuantity<IMaterial> leftMaterials,
            EconomicSubzoneHolding rightAnchor, 
            MultiQuantity<IMaterial> rightMaterials,
            List<Fleet> assignedFleets)
        {
            Faction = faction;
            LeftAnchor = leftAnchor;
            LeftMaterials = leftMaterials;
            RightAnchor = rightAnchor;
            RightMaterials = rightMaterials;
            AssignedFleets = assignedFleets;
        }
    }
}