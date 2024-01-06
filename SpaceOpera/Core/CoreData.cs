using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Cardamom.Json.OpenTK;
using Cardamom.Logging;
using Cardamom.Trackers;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Generator;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Politics.Governments;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core
{
    public class CoreData
    {
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BaseMaterial> Materials { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public MaterialSink? MaterialSink { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<GameModifier> Modifiers { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BaseAdvancement> Advancements { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Structure> Structures { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Recipe> Recipes { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Biome> Biomes { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<BaseComponent> Components { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public List<ComponentTypeClassifier> ComponentClassifiers { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<DesignTemplate> DesignTemplates { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public List<GovernmentForm> GovernmentForms { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public GalaxyGenerator? GalaxyGenerator { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public PoliticsGenerator? PoliticsGenerator { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public EconomyGenerator? EconomyGenerator { get; set; }

        public static CoreData LoadFrom(string path, ILogger logger)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = new KeyedReferenceHandler(),
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector3JsonConverter());
            options.Converters.Add(new Vector4JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            options.Converters.Add(new Matrix4JsonConverter());
            options.Converters.Add(
                new ReferenceDictionaryJsonConverter().CreateConverter(typeof(MultiQuantity<IMaterial>), options)!);
            var data = JsonSerializer.Deserialize<CoreData>(File.ReadAllText(path), options)!;
            logger = logger.ForType(typeof(CoreData)).AtInfo();
            logger.Log("Loaded");
            logger.Log($"\t{data.Advancements.Count} Advancements");
            logger.Log($"\t{data.Biomes.Count} Biomes");
            logger.Log($"\t{data.ComponentClassifiers.Count} ComponentClassifiers");
            logger.Log($"\t{data.Components.Count} Components");
            logger.Log($"\t{data.DesignTemplates.Count} DesignTemplates");
            logger.Log($"\t{data.GovernmentForms.Count} GovernmentForms");
            logger.Log($"\t{data.Materials.Count} Materials");
            logger.Log($"\t{data.MaterialSink != null} MaterialSink");
            logger.Log($"\t{data.Modifiers.Count} Modifiers");
            logger.Log($"\t{data.Recipes.Count} Recipes");
            logger.Log($"\t{data.Structures.Count} Structures");
            return data;
        }
    }
}