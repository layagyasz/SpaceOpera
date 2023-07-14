using Cardamom.Collections;
using SpaceOpera.Core.Politics.Government;

namespace SpaceOpera.Core.Politics
{
    public class Faction
    {
        public string Name { get; }
        public Banner Banner { get; }
        public GovernmentForm GovernmentForm { get; }
        public NameGenerator NameGenerator { get; }

        private readonly EnumMap<FactionAttribute, float> _attributes;

        public Faction(
            string name, 
            Banner banner, 
            GovernmentForm governmentForm,
            EnumMap<FactionAttribute, float> baseAttributes,
            NameGenerator nameGenerator)
        {
            Name = name;
            Banner = banner;
            GovernmentForm = governmentForm;
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