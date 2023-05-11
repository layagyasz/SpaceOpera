using Cardamom.Collections;

namespace SpaceOpera.Core.Politics
{
    public class Faction
    {
        public string Name { get; }
        public Banner Banner { get; }
        public NameGenerator NameGenerator { get; }

        private readonly EnumMap<FactionAttribute, float> _attributes;

        public Faction(
            string name, Banner banner, EnumMap<FactionAttribute, float> baseAttributes, NameGenerator nameGenerator)
        {
            Name = name;
            Banner = banner;
            NameGenerator = nameGenerator;

            _attributes = new EnumMap<FactionAttribute, float>(baseAttributes);
        }

        public float GetSpaceForcesCommand()
        {
            return _attributes[FactionAttribute.SpaceForcesCommand];
        }

        public float GetFleetCommand()
        {
            return _attributes[FactionAttribute.FleetCommand];
        }

        public float GetLandForcesCommand()
        {
            return _attributes[FactionAttribute.LandForcesCommand];
        }

        public float GetArmyCommand()
        {
            return _attributes[FactionAttribute.ArmyCommand];
        }
    }
}