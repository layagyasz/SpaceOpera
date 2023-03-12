using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Languages;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Politics
{
    public class NameGenerator
    {
        public Language Language { get; }
        public ComponentNameGenerator ComponentNameGenerator { get; }

        private readonly IIdGenerator _fleetIdGenerator = new SerialIdGenerator(1);

        public NameGenerator(Language language, ComponentNameGenerator componentNameGenerator)
        {
            Language = language;
            ComponentNameGenerator = componentNameGenerator;
        }

        public string GenerateNameFor(StarSystem starSystem, Random random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(starSystem), Language, random);
        }

        public string GenerateNameFor(StellarBody stellarBody, StarSystem starSystem, Random random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(stellarBody, starSystem), Language, random);
        }

        public string GenerateNameFor(StellarBodyRegion stellarBodyRegion, Random random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(stellarBodyRegion), Language, random);
        }

        public string GenerateNameFor(Design design, Random random)
        {
            return ComponentNameGenerator.GenerateNameFor(ToArgs(design), Language, random);
        }

        public string GenerateNameForFaction(Random random)
        {
            return ComponentNameGenerator.GenerateNameFor(GetFactionArgs(), Language, random);
        }

        public string GenerateNameForFleet(Random random)
        {
            return ComponentNameGenerator.GenerateNameFor(GetFleetArgs(), Language, random);
        }

        public string GenerateNameForStar(Random random)
        {
            return ComponentNameGenerator.GenerateNameFor(GetStarArgs(), Language, random);
        }

        private static NameGeneratorArgs GetFactionArgs()
        {
            return new(NameType.Faction);
        }

        private NameGeneratorArgs GetFleetArgs()
        {
            return new(NameType.Fleet) { SequenceNumber = _fleetIdGenerator.Generate() };
        }

        private static NameGeneratorArgs GetStarArgs()
        {
            return new(NameType.Star);
        }

        private static NameGeneratorArgs ToArgs(StarSystem starSystem)
        {
            return new(NameType.StarSystem) { ParentName = starSystem.Star.Name };
        }

        private static NameGeneratorArgs ToArgs(StellarBody stellarBody, StarSystem starSystem)
        {
            return new(NameType.StellarBody)
            {
                ParentName = starSystem.Star.Name,
                SequenceNumber = starSystem.Orbiters.IndexOf(stellarBody) + 1
            };
        }

        private static NameGeneratorArgs ToArgs(StellarBodyRegion stellarBodyRegion)
        {
            return new(NameType.StellarBodyRegion);
        }

        private static NameGeneratorArgs ToArgs(Design design)
        {
            return new(ToNameType(design.Configuration.Template.Type)) { Tags = design.Tags };
        }

        public static NameType ToNameType(ComponentType type)
        {
            return type switch
            {
                ComponentType.Ship => NameType.Ship,
                ComponentType.Infantry => NameType.Infantry,
                ComponentType.BattalionTemplate => NameType.BattalionTemplate,
                ComponentType.DivisionTemplate => NameType.DivisionTemplate,
                ComponentType.ShipWeapon 
                    or ComponentType.ShipPointDefense
                    or ComponentType.ShipMissile 
                    or ComponentType.PersonalWeapon => NameType.Weapon,
                ComponentType.ShipShield => NameType.Shield,
                _ => throw new ArgumentException($"Unsupported ComponentType: [{type}]."),
            };
        }
    }
}