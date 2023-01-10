using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics.Generator
{
    class FleetGenerator
    {
        public float InitialCommand { get; set; }

        public void Generate(World World, Faction Faction, INavigable Headquarters, Random Random)
        {
            int fleets = (int)Math.Ceiling(InitialCommand / Faction.GetFleetCommand());
            float perFleetCommand = InitialCommand / fleets;

            var shipDesigns = 
                World.GetDesignsFor(Faction)
                    .Where(x => x.Configuration.Template.Type == ComponentType.SHIP)
                    .SelectMany(x => x.Components)
                    .Cast<Unit>()
                    .ToArray();
            var shipDesignWeights = shipDesigns.Select(x => x.Command).ToArray();
            var totalCommand = shipDesignWeights.Sum();
            for (int i=0; i<shipDesignWeights.Length;  ++i)
            {
                shipDesignWeights[i] /= totalCommand;
            }
            var shipCounts = 
                shipDesignWeights.Select(
                    x => (int)(perFleetCommand / (totalCommand * shipDesignWeights.Length * x))).ToArray();
            var composition = new MultiCount<Unit>();
            for (int i=0; i<shipDesigns.Length; ++i)
            {
                composition.Add(shipDesigns[i], shipCounts[i]);
            }

            for (int i=0; i<fleets; ++i)
            {
                var fleet = new Fleet(Faction);
                fleet.SetName(Faction.NameGenerator.GenerateNameForFleet(Random));
                fleet.Add(composition);
                fleet.SetPosition(Headquarters);
                World.AddFleet(fleet);
            }
        }
    }
}