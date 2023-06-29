using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Designs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ComponentType
    {
        Unknown,

        BattalionTemplate,
        DivisionTemplate,

        Infantry,
        InfantryPlatform,

        PersonalArmor,
        PersonalArmorPlating,
        PersonalArmorUnderlay,
        
        SmallArm,
        SmallArmConstruction,
        SmallArmStyle,
        SmallArmAmmunition,
        SmallArmOptics,
        SmallArmAssembly,

        PersonalShield,

        ShieldCapacitor,
        ShieldEmitter,
        ShieldPowerSupply,
        ShieldMount,
        ShieldWeight,

        HeavyGun,
        HeavyGunAmmunition,
        HeavyGunAssembly,
        HeavyGunFireControl,
        HeavyGunMount,
        HeavyGunWeight,

        HeavyMissile,

        Ship,
        ShipArmor,
        ShipConstruction,
        ShipHangar,
        ShipInternal,
        ShipJumpDrive,
        ShipPowercore,
        ShipShield,
        ShipThruster,
    }
}