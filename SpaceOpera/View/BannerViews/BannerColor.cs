using System.Text.Json.Serialization;

namespace SpaceOpera.View.BannerViews
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BannerColor
    {
        None,
        Primary,
        Secondary,
        Symbol
    }
}
