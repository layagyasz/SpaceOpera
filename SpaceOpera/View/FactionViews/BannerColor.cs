using System.Text.Json.Serialization;

namespace SpaceOpera.View.FactionViews
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
