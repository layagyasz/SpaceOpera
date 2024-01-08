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

        HeavyShield,
        HeavyShieldCapacitor,
        HeavyShieldEmitter,
        HeavyShieldPowerSupply,
        HeavyShieldMount,
        HeavyShieldWeight,

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
        ShipThruster,

        Vehicle,
        VehicleArmor,
        VehicleConstruction,
        VehicleSuspension
    }
}