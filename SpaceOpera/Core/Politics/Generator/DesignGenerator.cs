using Cardamom.Json.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Designs;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Politics.Generator
{
    public class DesignGenerator
    {
        [JsonConverter(typeof(ListDictionaryJsonConverter))]
        public MultiCount<AutoDesignerParameters> DesignParameters { get; set; } = new();

        private List<ComponentType>? _designOrder;

        public void Generate(World world, Faction faction, Random random)
        {
            _designOrder ??= world.AutoDesigner.GetDesignOrder();
            foreach (var component in _designOrder)
            {
                var parameters = DesignParameters.FirstOrDefault(x => x.Key.Type == component);
                if (parameters.Key == null)
                {
                    continue;
                }
                var designConfig = 
                    world.AutoDesigner.CreateSeries(parameters.Key, world.GetComponentsFor(faction), random);
                var design = world.DesignBuilder.Build(designConfig);
                design.SetName(faction.NameGenerator.GenerateNameFor(design, random));
                world.AddDesign(design);
                world.AddLicense(new DesignLicense(faction, design));
            }
        }
    }
}
