using Cardamom.Trackers;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Politics.Generator
{
    public class SpaceForcesGenerator
    {
        public static void Generate(World world, Faction faction, INavigable headquarters, GeneratorContext context)
        {
            int fleets = (int)Math.Ceiling(faction.GetSpaceForcesCommand() / faction.GetFleetCommand());

            var shipDesigns = 
                world.GetDesignsFor(faction)
                    .Where(x => x.Configuration.Template.Type == ComponentType.Ship)
                    .SelectMany(x => x.Components)
                    .Cast<Unit>()
                    .OrderBy(x => -x.Command)
                    .ToArray();
            float perFleetCommand = faction.GetFleetCommand();
            var composition = new MultiCount<Unit>();
            for (int i=0; i< shipDesigns.Length; ++i)
            {
                int count = (int)Math.Round(perFleetCommand / (shipDesigns[i].Command * (shipDesigns.Length - i)));
                composition.Add(shipDesigns[i], count);
                perFleetCommand -= count * shipDesigns[i].Command;
            }

            for (int i=0; i<fleets; ++i)
            {
                var fleet = new Fleet(faction);
                fleet.SetName(faction.NameGenerator.GenerateNameForFleet(context.Random));
                fleet.Add(composition);
                fleet.SetPosition(headquarters);
                world.Formations.AddFleet(fleet);
            }
        }
    }
}