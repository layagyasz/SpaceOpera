using Cardamom.Collections;
using Cardamom.Graphics;
using Cardamom.Json;
using Cardamom.Json.Collections;
using SpaceOpera.Core;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Generator;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;
using System.Text.Json.Serialization;

namespace SpaceOpera
{
    public class GameData
    {
        public GraphicsResources? GraphicResources { get; set; }

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<BaseMaterial> Materials { get; } = new();
        public MaterialSink? MaterialSink { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<GameModifier> Modifiers { get; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<AdvancementType> AdvancementTypes { get; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BaseAdvancement> Advancements { get; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Structure> Structures { get; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Recipe> Recipes { get; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<Biome> Biomes { get; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<StellarBodyGenerator> StellarBodyGenerators { get; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<StarGenerator> StarGenerator { get; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<BaseComponent> Components { get; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<DesignTemplate> DesignTemplates { get; set; } = new();

        public GalaxyGenerator? GalaxyGenerator { get; set; }
        public PoliticsGenerator? PoliticsGenerator { get; set; }
        public EconomyGenerator? EconomyGenerator { get; set; }
    }
}