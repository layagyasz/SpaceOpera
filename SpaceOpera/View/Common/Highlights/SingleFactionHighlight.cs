using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.Common.Highlights
{
    public class SingleFactionHighlight : IHighlight
    {
        public EventHandler<EventArgs>? OnUpdated { get; set; }

        public bool Merge => true;
        public float BorderWidth => 1f;
        public Faction Faction { get; }
        public Color4 BorderColor { get; }
        public Color4 Color { get; }

        public SingleFactionHighlight(Faction faction, Color4 borderColor, Color4 color)
        {
            Faction = faction;
            BorderColor = borderColor;
            Color = color;
        }

        public bool Contains(object @object)
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

        public void Unhook() { }
    }
}