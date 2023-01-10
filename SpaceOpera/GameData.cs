using Cardamom.Interface;
using Cardamom.Serialization;
using SFML.Graphics;
using SpaceOpera.Core;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Generator;
using SpaceOpera.Core.Languages.Generator;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Universe;
using SpaceOpera.Core.Universe.Generator;
using SpaceOpera.JsonConverters;
using SpaceOpera.View;
using SpaceOpera.View.Banners;
using SpaceOpera.View.Icons;
using SpaceOpera.View.Spectra;
using SpaceOpera.View.StellarBodyViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera
{
    class GameData : IDataSource
    {
        public Dictionary<string, BaseMaterial> Materials { get; } = new Dictionary<string, BaseMaterial>();
        public MaterialSink MaterialSink { get; set; }
        public Dictionary<string, GameModifier> Modifiers { get; } = new Dictionary<string, GameModifier>();
        public Dictionary<string, AdvancementType> AdvancementTypes { get; }
            = new Dictionary<string, AdvancementType>();
        public Dictionary<string, BaseAdvancement> Advancements { get; } = new Dictionary<string, BaseAdvancement>();
        public Dictionary<string, Structure> Structures { get; } = new Dictionary<string, Structure>();
        public Dictionary<string, Recipe> Recipes { get; } = new Dictionary<string, Recipe>();
        public Dictionary<string, Biome> Biomes { get; } = new Dictionary<string, Biome>();
        public Dictionary<string, StellarBodyGenerator> StellarBodyGenerators { get; }
            = new Dictionary<string, StellarBodyGenerator>();
        public Dictionary<string, StarGenerator> StarGenerator { get; } = new Dictionary<string, StarGenerator>();
        public Dictionary<string, IconImage> IconImages { get; } = new Dictionary<string, IconImage>();

        public Dictionary<string, BaseComponent> Components { get; } = new Dictionary<string, BaseComponent>();
        public Dictionary<ComponentType, DesignTemplate> DesignTemplates { get; set; } 
            = new Dictionary<ComponentType, DesignTemplate>();
        public DesignBuilder DesignBuilder { get; set; }
        public AutoDesigner AutoDesigner { get; set; }

        public GalaxyGenerator GalaxyGenerator { get; set; }
        public PoliticsGenerator PoliticsGenerator { get; set; }
        public EconomyGenerator EconomyGenerator { get; set; }

        public Shader LightingShader { get; set; }
        public SimpleIconFactory SimpleIconFactory { get; set; }
        public DesignedComponentIconFactory DesignedComponentIconFactory { get; set; }
        public BannerViewFactory BannerViewFactory { get; set; }
        public StarViewFactory StarViewFactory { get; set; }
        public StellarBodyViewFactory StellarBodyViewFactory { get; set; }

        public static GameData FromPath(string Path)
        {
            GameData gameData = new GameData();

            ClassLibrary.Instance.ReadBlock(new ParseBlock(new ParseBlock[]
                {
                    ParseBlock.FromFile(Path + "/Theme/Fonts.blk"),
                        new ParseBlock(
                            "class<>",
                            "classes",
                            Enumerable.Repeat(Path + "/Theme/Base.blk", 1)
                                .Concat(Directory.EnumerateFiles(
                                    Path + "/Theme/Components", "*", SearchOption.AllDirectories))
                                .SelectMany(i => ParseBlock.FromFile(i).Break()))
                }));

            Load<BaseMaterial>(Path + "/Materials.json", gameData.Materials);
            gameData.MaterialSink = 
                LoadConfig<MaterialSink>(Path + "/MaterialSink.json", CreateOptionsFor(gameData, typeof(IMaterial)));
            Load<GameModifier>(
                Path + "/Modifiers.json", gameData.Modifiers, CreateOptionsFor(gameData, typeof(IMaterial)));
            Load<AdvancementType>(
                Path + "/AdvancementTypes.json", 
                gameData.AdvancementTypes, 
                CreateOptionsFor(gameData, typeof(IMaterial)));
            Load<BaseAdvancement>(
                Path + "/Advancements.json",
                gameData.Advancements, 
                CreateOptionsFor(gameData, typeof(AdvancementType)));
            Reconcile<IAdvancement, BaseAdvancement>(gameData.Advancements, x => x.Prerequisites);
            Load<Structure>(
                Path + "/Structures.json", gameData.Structures, CreateOptionsFor(gameData, typeof(IMaterial)));
            Load<Recipe>(
                Path + "/Recipes.json",
                gameData.Recipes,
                CreateOptionsFor(gameData, typeof(IMaterial), typeof(Structure)));
            Load<Biome>(
                Path + "/Biomes.json", 
                gameData.Biomes,
                CreateOptionsFor(gameData, typeof(GameModifier)));
            gameData.EconomyGenerator =
                LoadConfig<EconomyGenerator>(
                    Path + "/EconomyGenerator.json",
                    CreateOptionsFor(gameData, typeof(Biome), typeof(IMaterial)));
            LoadDirectory<StellarBodyGenerator>(
                Path + "/StellarBodyGenerators", 
                gameData.StellarBodyGenerators, 
                CreateOptionsFor(
                    gameData, 
                    typeof(Biome), 
                    typeof(ResourceGenerator),
                    typeof(IMaterial)));
            Load<StarGenerator>(Path + "/StarGenerators.json", gameData.StarGenerator);

            List<BannerBackground> bannerBackgrounds = 
                JsonSerializer.Deserialize<List<BannerBackground>>(
                    File.ReadAllText(Path + "/BannerBackgrounds.json"), CreateOptionsFor(gameData));
            gameData.BannerViewFactory = 
                new BannerViewFactory(
                    new SymbolRenderer(Directory.GetFiles(Path + "/Symbols"), 256, 1024), bannerBackgrounds);

            var shader = new Shader(null, null, Path + "/Shaders/star_frag.sh");
            var humanEyeSensitivity = LoadConfig<SpectrumSensitivity>(Path + "/HumanEyeSpectrumSensitivity.json");
            gameData.StarViewFactory = new StarViewFactory(shader, humanEyeSensitivity);

            var biomeRenderDetails = new Dictionary<string, BiomeRenderDetails>();
            Load<BiomeRenderDetails>(Path + "/BiomeRenderDetails.json", biomeRenderDetails);
            gameData.StellarBodyViewFactory = 
                new StellarBodyViewFactory(
                    gameData.Biomes.ToDictionary(x => x.Value, x => biomeRenderDetails[x.Key]),
                    gameData.StarViewFactory, 
                    humanEyeSensitivity);

            Load<IconImage>(Path + "/Icons.json", gameData.IconImages);
            var iconRenderDetails = new Dictionary<string, IconRenderDetails>();
            LoadDirectory<IconRenderDetails>(
                Path + "/IconRenderDetails", iconRenderDetails, CreateOptionsFor(gameData, typeof(IconImage)));
            var iconRenderer = new IconImageRenderer(gameData.IconImages, 64, 512);

            gameData.LightingShader = new Shader(null, null, "Resources/Shaders/lighting_frag.sh");
            var designedComponentRenderDetails = 
                LoadDistributedList<DesignedComponentIconRenderDetails>(
                    Path + "/Designs", "*_renderDetails.json", CreateOptionsFor(gameData, typeof(IconImage)));
            gameData.SimpleIconFactory =
                    new SimpleIconFactory(iconRenderer, ClassLibrary.Instance.GetFont("abel"), iconRenderDetails);
            gameData.DesignedComponentIconFactory =
                    new DesignedComponentIconFactory(
                        iconRenderer, designedComponentRenderDetails.ToEnumMap(x => x.Type, x => x));


            LoadDistributed<BaseComponent>(
                Path + "/Designs", 
                "*_components.json", 
                gameData.Components, 
                CreateOptionsFor(gameData, typeof(IMaterial)));

            var templates = new Dictionary<string, DesignTemplate>();
            LoadDistributed<DesignTemplate>(
                Path + "/Designs", 
                "*_template.json",
                templates,
                CreateOptionsFor(gameData, typeof(Structure)));
            foreach (var template in templates.Values)
            {
                gameData.DesignTemplates.Add(template.Type, template);
            }
            gameData.DesignBuilder =
                new DesignBuilder(
                    new ComponentClassifier(
                        LoadDistributedList<ComponentTypeClassifier>(Path + "/Designs", "*_classification.json")));
            gameData.AutoDesigner = new AutoDesigner(templates.Values);

            gameData.GalaxyGenerator = 
                LoadConfig<GalaxyGenerator>(
                    Path + "/GalaxyGenerator.json",
                    CreateOptionsFor(gameData, typeof(StarGenerator), typeof(StellarBodyGenerator)));

            gameData.PoliticsGenerator =
                LoadConfig<PoliticsGenerator>(Path + "/PoliticsGenerator.json", CreateOptionsFor(gameData));

            return gameData;
        }

        static T LoadConfig<T>(string Path, JsonSerializerOptions Options = null)
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(Path), Options);
        }

        static List<T> LoadList<T>(string Path, JsonSerializerOptions Options = null)
        {
            return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(Path), Options);
        }

        static List<T> LoadDistributedList<T>(string Path, string Pattern , JsonSerializerOptions Options = null)
        {
            var result = new List<T>();
            foreach (string file in Directory.GetFiles(Path, Pattern, SearchOption.AllDirectories))
            {
                result.AddRange(LoadList<T>(file, Options));
            }
            return result;
        }

        static void Load<T>(
            string Path, Dictionary<string, T> Destination, JsonSerializerOptions Options = null) where T : IKeyed
        {
            List<T> newElements = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(Path), Options);

            foreach (var element in newElements)
            {
                Destination.Add(element.Key, element);
            }
        }

        static void LoadDirectory<T>(
            string Path, Dictionary<string, T> Destination, JsonSerializerOptions Options = null) where T : IKeyed
        {
            foreach (string file in Directory.GetFiles(Path))
            {
                Load<T>(file, Destination, Options);
            }
        }

        static void LoadDistributed<T>(
            string Path, 
            string Pattern,
            Dictionary<string, T> Destination,
            JsonSerializerOptions Options = null) where T : IKeyed
        {
            foreach (string file in Directory.GetFiles(Path, Pattern, SearchOption.AllDirectories))
            {
                Load<T>(file, Destination, Options);
            }
        }

        static void Reconcile<T, K>(Dictionary<string, K> Map, Func<K, List<T>> SelfReferenceFieldFn) 
            where T : IKeyed where K : T
        {
            foreach (var row in Map)
            {
                List<T> field = SelfReferenceFieldFn(row.Value);
                List<T> newValues = field.Select(x => Map[x.Key]).Cast<T>().ToList();
                field.Clear();
                field.AddRange(newValues);
            }
        }

        static JsonSerializerOptions CreateOptionsFor(IDataSource DataSource, params Type[] Types)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2fJsonConverter());
            options.Converters.Add(new MultiQuantityJsonConverter<IMaterial>());
            foreach (Type type in Types)
            {
                if (type == typeof(AdvancementType))
                {
                    options.Converters.Add(new ReferenceJsonConverter<AdvancementType>(DataSource));
                }
                if (type == typeof(IAdvancement))
                {
                    options.Converters.Add(new ReferenceJsonConverter<IAdvancement>(DataSource));
                }
                if (type == typeof(IMaterial))
                { 
                     options.Converters.Add(new ReferenceJsonConverter<IMaterial>(DataSource));
                }
                if (type == typeof(Biome))
                {
                    options.Converters.Add(new ReferenceJsonConverter<Biome>(DataSource));
                }
                if (type == typeof(GameModifier))
                {
                    options.Converters.Add(new ReferenceJsonConverter<GameModifier>(DataSource));
                }
                if (type == typeof(StarGenerator))
                {
                    options.Converters.Add(new ReferenceJsonConverter<StarGenerator>(DataSource));
                }
                if (type == typeof(StellarBodyGenerator))
                {
                    options.Converters.Add(new ReferenceJsonConverter<StellarBodyGenerator>(DataSource));
                }
                if (type == typeof(Structure))
                {
                    options.Converters.Add(new ReferenceJsonConverter<Structure>(DataSource));
                }
                if (type == typeof(Recipe))
                {
                    options.Converters.Add(new ReferenceJsonConverter<Recipe>(DataSource));
                }
                if (type == typeof(IconImage))
                {
                    options.Converters.Add(new ReferenceJsonConverter<IconImage>(DataSource));
                }
            }
            return options;
        }

        public object Get(Type Type, string Key)
        {
            if (Type == typeof(AdvancementType))
            {
                return AdvancementTypes[Key];
            }
            if (Type == typeof(IAdvancement))
            {
                return Advancements[Key];
            }
            if (Type == typeof(IMaterial))
            {
                return Materials[Key];
            }
            if (Type == typeof(Biome))
            {
                return Biomes[Key];
            }
            if (Type == typeof(GameModifier))
            {
                return Modifiers[Key];
            }
            if (Type == typeof(StarGenerator))
            {
                return StarGenerator[Key];
            }
            if (Type == typeof(StellarBodyGenerator))
            {
                return StellarBodyGenerators[Key];
            }
            if (Type == typeof(Structure))
            {
                return Structures[Key];
            }
            if (Type == typeof(Recipe))
            {
                return Recipes[Key];
            }
            if (Type == typeof(IconImage))
            {
                return IconImages[Key];
            }
            throw new ArgumentException(string.Format("Unhandled type: {0}", Type));
        }
    }
}