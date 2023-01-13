using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Military
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DamageType
    {
        None,
        Kinetic,
        Energy,
        ElectroMagnetic
    }
}
