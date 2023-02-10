using Cardamom;
using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Json.OpenTK;
using SpaceOpera.Core.Universe.Spectra;
using SpaceOpera.Views.StellarBodyViews;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera.Views
{
    public class ViewData
    {
        [JsonConverter(typeof(FromFileJsonConverter))]
        public GameResources? GameResources { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BiomeRenderDetails> BiomeRenderDetails { get; set; } = new();

        [JsonConverter(typeof(FromFileJsonConverter))]
        public SpectrumSensitivity? HumanEyeSensitivity { get; set; }

        public static ViewData LoadFrom(string path)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = new KeyedReferenceHandler()
            };
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new Vector2JsonConverter());
            options.Converters.Add(new Vector2iJsonConverter());
            return JsonSerializer.Deserialize<ViewData>(File.ReadAllText(path), options)!;
        }
    }
}
