using Cardamom.Trackers;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Politics.Generator
{
    public class LandForcesGenerator
    {
        public static void Generate(World world, Faction faction, INavigable headquarters, GeneratorContext context)
        {
            int armies = (int)Math.Ceiling(faction.GetLandForcesCommand() / faction.GetArmyCommand());

            var divisionTemplates =
                world.GetDesignsFor(faction)
                    .Where(x => x.Configuration.Template.Type == ComponentType.DivisionTemplate)
                    .SelectMany(x => x.Components)
                    .Cast<DivisionTemplate>()
                    .OrderBy(x => -x.Command)
                    .ToArray();
            float perArmyCommand = faction.GetArmyCommand();
            var composition = new MultiCount<DivisionTemplate>();
            for (int i = 0; i < divisionTemplates.Length; ++i)
            {
                int count = 
                    (int)Math.Round(perArmyCommand / (divisionTemplates[i].Command * (divisionTemplates.Length - i)));
                composition.Add(divisionTemplates[i], count);
                perArmyCommand -= count * divisionTemplates[i].Command;
            }

            for (int i = 0; i < armies; ++i)
            {
                var army = new Army(faction);
                army.SetName(faction.NameGenerator.GenerateNameForArmy(context.Random));
                foreach (var template in composition)
                {
                    for (int j=0; j< template.Value; ++j)
                    {
                        var division = new Division(faction, template.Key);
                        division.Add(template.Key.Composition);
                        division.SetName(faction.NameGenerator.GenerateNameFor(template.Key, context.Random));
                        division.SetPosition(headquarters);
                        world.FormationManager.AddDivision(division);
                        army.Divisions.Add(division);
                    }
                }
                world.FormationManager.AddArmy(army);
            }
        }
    }
}