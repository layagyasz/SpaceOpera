using SpaceOpera.Core.Designs;

namespace SpaceOpera.View
{
    public static class EnumMapper
    {
        public static string ToString(ComponentType componentType)
        {
            return componentType switch
            {
                ComponentType.Unknown => "NA",
                ComponentType.InfantryPlatform => "Infantry Platform",
                ComponentType.PersonalArmor => "Personal Armor",
                ComponentType.PersonalArmorPlating => "Personal Armor Plating",
                ComponentType.PersonalArmorUnderlay => "Personal Armor Underlay",
                ComponentType.PersonalWeapon => "Personal Weapon",
                ComponentType.PersonalWeaponConstruction => "Personal Weapon Construction",
                ComponentType.PersonalWeaponStyle => "Personal Weapon Style",
                ComponentType.PersonalWeaponAmmunition => "Personal Weapon Ammunition",
                ComponentType.PersonalWeaponOptics => "Personal Weapon Optics",
                ComponentType.PersonalWeaponAssembly => "Personal Weapon Assembly",
                ComponentType.PersonalShield => "Personal Shield",
                ComponentType.ShieldCapacitor => "Shield Capacitor",
                ComponentType.ShieldEmitter => "Shield Emitter",
                ComponentType.ShieldPowerSupply => "Shield Power Supply",
                ComponentType.ShieldMount => "Shield Mount",
                ComponentType.ShieldWeight => "Shield Weight",
                ComponentType.ShipWeaponAmmunition => "Ship Weapon Ammunition",
                ComponentType.ShipWeaponAssembly => "Ship Weapon Assembly",
                ComponentType.ShipWeaponFireControl => "Ship Weapon Fire Control",
                ComponentType.ShipWeaponMount => "Ship Weapon Mount",
                ComponentType.ShipWeaponWeight => "Ship Weapon Weight",
                ComponentType.Ship => "Ship",
                ComponentType.ShipArmor => "Ship Armor",
                ComponentType.ShipConstruction => "Ship Construction",
                ComponentType.ShipHangar => "Ship Hangar",
                ComponentType.ShipInternal => "Ship Internal",
                ComponentType.ShipJumpDrive => "Ship Jump Drive",
                ComponentType.ShipMissile => "Ship Missile",
                ComponentType.ShipPointDefense => "Ship Point Defense",
                ComponentType.ShipPowercore => "Ship Powercore",
                ComponentType.ShipShield => "Ship Shield",
                ComponentType.ShipThruster => "Ship Thruster",
                ComponentType.ShipWeapon => "Ship Weapon",
                ComponentType.Infantry => "Infantry",
                ComponentType.BattalionTemplate => "Battalion",
                ComponentType.DivisionTemplate => "Division",
                _ => throw new ArgumentException($"Unsupported component type [{componentType}]"),
            };
        }
    }
}
