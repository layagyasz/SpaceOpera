using Cardamom.Collections;
using SpaceOpera.Core.Politics.Cultures;
using SpaceOpera.Core.Politics.Governments;

namespace SpaceOpera.Core.Politics
{
    public class Faction
    {
        public string Name { get; }
        public Banner Banner { get; }
        public Culture Culture { get; }
        public GovernmentForm GovernmentForm { get; }
        public NameGenerator NameGenerator { get; }

        private readonly EnumMap<FactionAttribute, float> _attributes;

        public Faction(
            string name, 
            Banner banner, 
            Culture culture,
            GovernmentForm governmentForm,
            EnumMap<FactionAttribute, float> baseAttributes,
            NameGenerator nameGenerator)
        {
            Name = name;
            Banner = banner;
            Culture = culture;
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

        public int GetResearchOptions()
        {
            return (int)_attributes[FactionAttribute.ResearchOptions];
        }

        public override string ToString()
        {
            return Name;
        }
    }
}