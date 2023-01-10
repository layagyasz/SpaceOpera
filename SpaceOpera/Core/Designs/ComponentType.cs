using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    enum ComponentType
    {
        NONE,

        INFANTRY_PLATFORM,

        PERSONAL_ARMOR,
        PERSONAL_ARMOR_PLATING,
        PERSONAL_ARMOR_UNDERLAY,
        PERSONAL_ARMOR_HEAD,
        PERSONAL_ARMOR_BODY,
        PERSONAL_ARMOR_ARMS,
        PERSONAL_ARMOR_LEGS,
        
        PERSONAL_WEAPON,
        PERSONAL_WEAPON_CONSTRUCTION,
        PERSONAL_WEAPON_STYLE,
        PERSONAL_WEAPON_AMMO,
        PERSONAL_WEAPON_OPTICS,
        PERSONAL_WEAPON_ASSEMBLY,

        PERSONAL_SHIELD,

        SHIELD_CAPACITOR,
        SHIELD_EMITTER,
        SHIELD_POWER_SUPPLY,
        SHIELD_MOUNT,
        SHIELD_WEIGHT,

        SHIP_WEAPON_AMMUNITION,
        SHIP_WEAPON_ASSEMBLY,
        SHIP_WEAPON_FIRE_CONTROL,
        SHIP_WEAPON_MOUNT,
        SHIP_WEAPON_WEIGHT,

        SHIP,
        SHIP_ARMOR,
        SHIP_CONSTRUCTION,
        SHIP_HANGAR,
        SHIP_INTERNAL,
        SHIP_JUMP_DRIVE,
        SHIP_MISSLE,
        SHIP_POINT_DEFENSE,
        SIHP_POWER_CORE,
        SHIP_SHIELD,
        SHIP_THRUSTER,
        SHIP_WEAPON,

        INFANTRY,
        BATTALION_TEMPLATE,
        DIVISION_TEMPLATE
    }
}