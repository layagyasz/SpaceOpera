using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComponentType
    {
        Unknown,

        InfantryPlatform,

        PersonalArmor,
        PersonalArmorPlating,
        PersonalArmorUnderlay,
        
        PersonalWeapon,
        PersonalWeaponConstruction,
        PersonalWeaponStyle,
        PersonalWeaponAmmunition,
        PersonalWeaponOptics,
        PersonalWeaponAssembly,

        PersonalShield,

        ShieldCapacitor,
        ShieldEmitter,
        ShieldPowerSupply,
        ShieldMount,
        ShieldWeight,

        ShipWeaponAmmunition,
        ShipWeaponAssembly,
        ShipWeaponFireControl,
        ShipWeaponMount,
        ShipWeaponWeight,

        Ship,
        ShipArmor,
        ShipConstruction,
        ShipHangar,
        ShipInternal,
        ShipJumpDrive,
        ShipMissile,
        ShipPointDefense,
        ShipPowercore,
        ShipShield,
        ShipThruster,
        ShipWeapon,

        Infantry,
        BattalionTemplate,
        DivisionTemplate
    }
}