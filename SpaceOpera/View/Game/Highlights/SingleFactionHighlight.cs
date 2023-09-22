using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Game.Highlights
{
    public class SingleFactionHighlight : RegionHighlight
    {
        public override EventHandler<EventArgs>? Updated { get; set; }

        public override bool Merge => true;
        public override float BorderWidth => 4f;
        public override Color4 BorderColor { get; }
        public override Color4 Color { get; }

        public Faction Faction { get; }

        public SingleFactionHighlight(Faction faction, Color4 borderColor, Color4 color)
        {
            Faction = faction;
            BorderColor = borderColor;
            Color = color;
        }

        public override bool Contains(object @object)
        {
            if (@object is StarSystem system)
            {
                return system.ContainsFaction(Faction);
            }
            if (@object is LocalOrbitRegion node)
            {
                return node.StellarBody.ContainsFaction(Faction);
            }
            if (@object is StellarBodySubRegion region)
            {
                return region.ParentRegion!.Sovereign == Faction;
            }
            return false;
        }

        public override void Hook(object domain) { }

        public override void Unhook(object domain) { }
    }
}