using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Languages;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics
{
    class NameGenerator
    {
        public Language Language { get; }
        public ComponentNameGenerator ComponentNameGenerator { get; }
        public ComponentTypeNameGenerator FormationNameGenerator { get; }

        private IIdGenerator _FleetIdGenerator = new SerialIdGenerator(1);

        public NameGenerator(Language Language, ComponentNameGenerator ComponentNameGenerator)
        {
            this.Language = Language;
            this.ComponentNameGenerator = ComponentNameGenerator;
        }

        public string GenerateNameFor(StarSystem StarSystem, Random Random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(StarSystem), Language, Random);
        }

        public string GenerateNameFor(StellarBody StellarBody, StarSystem StarSystem, Random Random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(StellarBody, StarSystem), Language, Random);
        }

        public string GenerateNameFor(StellarBodyRegion StellarBodyRegion, Random Random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(StellarBodyRegion), Language, Random);
        }

        public string GenerateNameFor(Design Design, Random Random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(Design), Language, Random);
        }

        public string GenerateNameForFaction(Random Random)
        {
            return ComponentNameGenerator.GenerateNameFor(GetFactionArgs(), Language, Random);
        }

        public string GenerateNameForFleet(Random Random)
        {
            return ComponentNameGenerator.GenerateNameFor(GetFleetArgs(), Language, Random);
        }

        public string GenerateNameForStar(Random Random)
        {
            return ComponentNameGenerator.GenerateNameFor(GetStarArgs(), Language, Random);
        }

        private NameGeneratorArgs GetFactionArgs()
        {
            return new NameGeneratorArgs(NameType.FACTION);
        }

        private NameGeneratorArgs GetFleetArgs()
        {
            return new NameGeneratorArgs(NameType.FLEET) { SequenceNumber = _FleetIdGenerator.Generate() };
        }

        private NameGeneratorArgs GetStarArgs()
        {
            return new NameGeneratorArgs(NameType.STAR);
        }

        private NameGeneratorArgs ToArgs(StarSystem StarSystem)
        {
            return new NameGeneratorArgs(NameType.STAR_SYSTEM) { ParentName = StarSystem.Star.Name };
        }

        private NameGeneratorArgs ToArgs(StellarBody StellarBody, StarSystem StarSystem)
        {
            return new NameGeneratorArgs(NameType.STELLAR_BODY)
            {
                ParentName = StarSystem.Star.Name,
                SequenceNumber = StarSystem.Orbiters.IndexOf(StellarBody) + 1
            };
        }

        private NameGeneratorArgs ToArgs(StellarBodyRegion StellarBodyRegion)
        {
            return new NameGeneratorArgs(NameType.STELLAR_BODY_REGION);
        }

        private NameGeneratorArgs ToArgs(Design Design)
        {
            return new NameGeneratorArgs(ToNameType(Design.Configuration.Template.Type)) { Tags = Design.Tags };
        }

        public static NameType ToNameType(ComponentType Type)
        {
            switch (Type)
            {
                case ComponentType.SHIP:
                    return NameType.SHIP;
                case ComponentType.INFANTRY:
                    return NameType.INFANTRY;
                case ComponentType.BATTALION_TEMPLATE:
                    return NameType.BATTALION_TEMPLATE;
                case ComponentType.DIVISION_TEMPLATE:
                    return NameType.DIVISION_TEMPLATE;
                case ComponentType.SHIP_WEAPON:
                case ComponentType.SHIP_POINT_DEFENSE:
                case ComponentType.SHIP_MISSLE:
                case ComponentType.PERSONAL_WEAPON:
                    return NameType.WEAPON;
                case ComponentType.SHIP_SHIELD:
                    return NameType.SHIELD;
                default:
                    throw new ArgumentException();
            }
        }
    }
}