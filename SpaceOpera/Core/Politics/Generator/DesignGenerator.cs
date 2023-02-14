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

        public void Generate(World world, Faction faction, GeneratorContext context)
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
                    world.AutoDesigner.CreateSeries(parameters.Key, world.GetComponentsFor(faction), context.Random);
                designConfig.SetName(faction.NameGenerator.GenerateNameFor(designConfig, context.Random));
                var design = world.DesignBuilder.Build(designConfig);
                world.AddDesign(design);
                world.AddLicense(new(faction, design));
            }
        }
    }
}
