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
using SpaceOpera.Core.Universe.Spectra;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera
{
    public class GameData
    {
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
        [JsonConverter(typeof(FromFileJsonConverter))]
        public GalaxyGenerator? GalaxyGenerator { get; set; }

        [JsonPropertyOrder(13)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public PoliticsGenerator? PoliticsGenerator { get; set; }

        [JsonPropertyOrder(14)]
        [JsonConverter(typeof(FromFileJsonConverter))]
        public EconomyGenerator? EconomyGenerator { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public SpectrumSensitivity? HumanEyeSensitivity { get; set; }

        public static GameData LoadFrom(string path)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = new KeyedReferenceHandler(),
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector3JsonConverter());
            options.Converters.Add(new Vector4JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            options.Converters.Add(new Matrix4JsonConverter());
            return JsonSerializer.Deserialize<GameData>(File.ReadAllText(path), options)!;
        }
    }
}