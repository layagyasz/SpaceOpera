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
        HeavyGunMount,
        HeavyGunOptic,
        HeavyGunWeight,

        HeavyMissile,
        Internal,

        Ship,
        ShipArmor,
        ShipConstruction,
        ShipHangar,
        ShipJumpDrive,
        ShipPowercore,
        ShipThruster,

        Vehicle,
        VehicleArmor,
        VehicleConstruction,
        VehicleSuspension
    }
}