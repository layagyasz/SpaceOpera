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
        PersonalArmorHead,
        PersonalArmorBody,
        PersonalArmorArms,
        PersonalArmorLegs,
        
        PersonalWeapon,
        PersonalWeaponConstruction,
        PersonalWeaponStyle,
        PersonalWeaponAmmo,
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
        ShipPowerCore,
        ShipShield,
        ShipThruster,
        ShipWeapon,

        Infantry,
        BattalionTemplate,
        DivisionTemplate
    }
}