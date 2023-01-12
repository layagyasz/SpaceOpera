using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComponentTag
    {
        None,

        Light,
        Medium,
        Heavy,

        SmallSize,
        MediumSize,
        LargeSize,

        Motorized,
        Mechanized,
        SelfPropelled,

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