using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComponentTag
    {
        Unknown,

        Light,
        Medium,
        Heavy,

        SmallSize,
        MediumSize,
        LargeSize,

        Motorized,
        Mechanized,
        SelfPropelled,

        Legged,
        Wheeled,
        Tracked,

        AntiAir,
        AntiArmor,
        Artillery,
        Engineer,
        Infantry,
        Tank,

        Battleship,
        Carrier,
        Cruiser,
        Destroyer,
        Escort,
        Freighter,
        Frigate,
        Patrol,
        Transport
    }
}