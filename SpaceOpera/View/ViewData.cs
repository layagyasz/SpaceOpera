using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Cardamom.Json.OpenTK;
using Cardamom.Logging;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.View.BannerViews;
using SpaceOpera.View.Game.StellarBodyViews;
using SpaceOpera.View.Icons;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera.View
{
    public class ViewData
    {
        [JsonConverter(typeof(FromFileJsonConverter))]
        public GameResources? GameResources { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public BannerViewFactory? Banners { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BiomeRenderDetails> Biomes { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public SpectrumSensitivity? HumanEyeSensitivity { get; set; }

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public Library<StaticIconConfig> Icons { get; set; } = new();

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public List<DesignedComponentIconConfig> DesignedComponentIconConfigs { get; set; } = new();

        public static ViewData LoadFrom(string path, ILogger logger)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = new KeyedReferenceHandler()
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            var data = JsonSerializer.Deserialize<ViewData>(File.ReadAllText(path), options)!;
            logger = logger.ForType(typeof(ViewData)).AtInfo();
            logger.Log("Loaded");
            logger.Log($"\t{data.GameResources != null} GameResources");
            logger.Log($"\t{data.Banners != null} Banners");
            logger.Log($"\t{data.Biomes.Count} Biomes");
            logger.Log($"\t{data.HumanEyeSensitivity != null} HumanEyeSensitivity");
            logger.Log($"\t{data.Icons.Count} Icons");
            logger.Log($"\t{data.DesignedComponentIconConfigs.Count} DesignedComponentIconConfigs");
            return data;
        }
    }
}
