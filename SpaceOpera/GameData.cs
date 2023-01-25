using Cardamom;
using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Cardamom.Json.OpenTK;
using SpaceOpera.Core;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Generator;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera
{
    public class GameData
    {
        public GraphicsResources? GraphicResources { get; set; }

        [JsonPropertyOrder(1)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BaseMaterial> Materials { get; set; } = new();

        [JsonPropertyOrder(2)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public MaterialSink? MaterialSink { get; set; }

        [JsonPropertyOrder(3)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<GameModifier> Modifiers { get; set; } = new();

        [JsonPropertyOrder(4)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<AdvancementType> AdvancementTypes { get; set; } = new();

        [JsonPropertyOrder(5)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BaseAdvancement> Advancements { get; set; } = new();

        [JsonPropertyOrder(6)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Structure> Structures { get; set; } = new();

        [JsonPropertyOrder(7)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Recipe> Recipes { get; set; } = new();

        [JsonPropertyOrder(8)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Biome> Biomes { get; set; } = new();
        
        [JsonPropertyOrder(9)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<BaseComponent> Components { get; set; } = new();

        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public List<ComponentTypeClassifier> ComponentClassifiers { get; set; } = new();

        [JsonPropertyOrder(11)]
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<DesignTemplate> DesignTemplates { get; set; } = new();

        [JsonPropertyOrder(12)]
        public GalaxyGenerator? GalaxyGenerator { get; set; }
        [JsonPropertyOrder(13)]
        public PoliticsGenerator? PoliticsGenerator { get; set; }
        [JsonPropertyOrder(14)]
        public EconomyGenerator? EconomyGenerator { get; set; }

        public static GameData LoadFrom(string path)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = new KeyedReferenceHandler(new Dictionary<string, IKeyed>())
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            return JsonSerializer.Deserialize<GameData>(File.ReadAllText(path), options)!;
        }
    }
}