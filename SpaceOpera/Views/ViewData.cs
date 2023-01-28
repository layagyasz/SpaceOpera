using Cardamom.Collections;
using Cardamom.Json;
using Cardamom.Json.OpenTK;
using SpaceOpera.Views.StellarBodyViews;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpaceOpera.Views
{
    public class ViewData
    {
        [JsonConverter(typeof(FromFileJsonConverter))]
        public Library<BiomeRenderDetails> BiomeRenderDetails { get; set; } = new();

        public static ViewData LoadFrom(string path)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = new KeyedReferenceHandler()
            };
            options.Converters.Add(new ColorJsonConverter());
            return JsonSerializer.Deserialize<ViewData>(File.ReadAllText(path), options)!;
        }
    }
}
