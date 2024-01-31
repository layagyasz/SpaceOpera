using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComponentAttribute
    {
        Unknown,

        // Generic
        DamageResist,
        Size,
        ProductionCost,
        MaterialCost,
        Durability,
        Hitpoints,
        Threat,
        Command,
        Detection,
        Evasion,
        Maneuver,
        Speed,

        // Armor
        ArmorProtection,
        ArmorThickness,
        ArmorCoverage,

        // Shield
        ShieldAbsorption,
        ShieldCapacity,
        ShieldRecharge,

        // Weapon
        WeaponDamage,
        WeaponAccuracy,
        WeaponTracking,
        WeaponPenetration,
           
        // Vehicle
        CargoSpace,
        HangarSpace,
        PassengerSpace
    }
}