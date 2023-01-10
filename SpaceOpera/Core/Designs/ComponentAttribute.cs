using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Designs
{
    enum ComponentAttribute
    {
        // Generic
        DAMAGE_RESIST,
        SIZE,
        PRODUCTION_COST,
        MATERIAL_COST,
        DURABILITY,
        HITPOINTS,
        THREAT,
        COMMAND,
        DETECTION,
        EVASION,
        MANEUVER,
        SPEED,

        // Armor
        ARMOR_PROTECTION,
        ARMOR_THICKNESS,
        ARMOR_COVERAGE,

        // Shield
        SHIELD_ABSORPTION,
        SHIELD_CAPACITY,
        SHIELD_RECHARGE,

        // Weapon
        WEAPON_DAMAGE,
        WEAPON_ACCURACY,
        WEAPON_TRACKING,
        WEAPON_PENETRATION,
           
        // Ship
        CARGO_SPACE,
        HANGAR_SPACE
    }
}