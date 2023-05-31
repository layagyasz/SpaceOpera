using Cardamom.Trackers;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics
{
    public class PersistentRoute
    {
        public Faction Faction { get; set; }
        public EconomicSubzone LeftAnchor { get; }
        public MultiQuantity<IMaterial> LeftMaterials { get; }
        public EconomicSubzone RightAnchor { get; set; }
        public MultiQuantity<IMaterial> RightMaterials { get; }
        public List<Fleet> AssignedFleets { get; set; }

        public PersistentRoute(
            Faction faction,
            EconomicSubzone leftAnchor, 
            MultiQuantity<IMaterial> leftMaterials,
            EconomicSubzone rightAnchor, 
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