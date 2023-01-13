using SpaceOpera.Core.Economics;

namespace SpaceOpera.Core
{
    public class SingleGameModifier
    {
        public ModifierType Type { get; set; }
        public IMaterial? Material { get; set; }
        public Modifier Modifier { get; set; }
    }
}
