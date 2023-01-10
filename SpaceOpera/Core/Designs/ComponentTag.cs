using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    enum ComponentTag
    {
        UNKNOWN,

        LIGHT,
        MEDIUM,
        HEAVY,

        SMALL_SIZE,
        MEDIUM_SIZE,
        LARGE_SIZE,

        MOTORIZED,
        MECHANIZED,
        SELF_PROPELLED,

        ANTI_AIR,
        ANTI_ARMOR,
        ARTILLERY,
        ENGINEER,
        INFANTRY,
        TANK,

        BATTLESHIP,
        CARRIER,
        CRUISER,
        DESTROYER,
        ESCORT,
        FREIGHTER,
        FRIGATE,
        PATROL,
        TRANSPORT
    }
}