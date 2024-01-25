using Cardamom.Collections;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Politics;
using static OpenTK.Graphics.OpenGL.GL;

namespace SpaceOpera.View
{
    public static class EnumMapper
    {
        public static string ToString(ComponentType componentType)
        {
            return componentType switch
            {
                ComponentType.Unknown => "NA",
                ComponentType.BattalionTemplate => "Battalion",
                ComponentType.DivisionTemplate => "Division",
                ComponentType.HeavyGun => "Heavy Gun",
                ComponentType.HeavyGunAmmunition => "Heavy Gun Ammunition",
                ComponentType.HeavyGunAssembly => "Heavy Gun Assembly",
                ComponentType.HeavyGunMount => "Heavy Gun Mount",
                ComponentType.HeavyGunOptic => "Heavy Gun Optic",
                ComponentType.HeavyGunWeight => "Heavy Gun Weight",
                ComponentType.HeavyMissile => "Heavy Missile",
                ComponentType.HeavyShield => "Ship Shield",
                ComponentType.HeavyShieldCapacitor => "Shield Capacitor",
                ComponentType.HeavyShieldEmitter => "Shield Emitter",
                ComponentType.HeavyShieldPowerSupply => "Shield Power Supply",
                ComponentType.HeavyShieldMount => "Shield Mount",
                ComponentType.HeavyShieldWeight => "Shield Weight",
                ComponentType.Infantry => "Infantry",
                ComponentType.InfantryPlatform => "Infantry Platform",
                ComponentType.Internal => "Internal",
                ComponentType.PersonalArmor => "Personal Armor",
                ComponentType.PersonalArmorPlating => "Personal Armor Plating",
                ComponentType.PersonalArmorUnderlay => "Personal Armor Underlay",
                ComponentType.PersonalShield => "Personal Shield",
                ComponentType.Ship => "Ship",
                ComponentType.ShipArmor => "Ship Armor",
                ComponentType.ShipConstruction => "Ship Construction",
                ComponentType.ShipHangar => "Ship Hangar",
                ComponentType.ShipJumpDrive => "Ship Jump Drive",
                ComponentType.ShipPowercore => "Ship Powercore",
                ComponentType.ShipThruster => "Ship Thruster",
                ComponentType.SmallArm => "Small Arm",
                ComponentType.SmallArmConstruction => "Small Arm Construction",
                ComponentType.SmallArmStyle => "Small Arm Style",
                ComponentType.SmallArmAmmunition => "Small Arm Ammunition",
                ComponentType.SmallArmOptics => "Small Arm Optics",
                ComponentType.SmallArmAssembly => "Small Arm Assembly",
                ComponentType.Vehicle => "Vehicle",
                ComponentType.VehicleArmor => "Vehicle Armor",
                ComponentType.VehicleConstruction => "Vehicle Construction",
                ComponentType.VehicleSuspension => "Vehicle Suspension",
                _ => throw new ArgumentException($"Unsupported component type [{componentType}]"),
            };
        }

        public static string ToString(DiplomaticRelation.DiplomaticStatus status)
        {
            return status switch
            {
                DiplomaticRelation.DiplomaticStatus.Peace => "Peace",
                DiplomaticRelation.DiplomaticStatus.War => "War",
                DiplomaticRelation.DiplomaticStatus.None => "NA",
                _ => throw new ArgumentException($"Unsupported status [{status}]"),
            };
        }

        public static string ToString(ProjectStatus status)
        {
            return status switch
            {
                ProjectStatus.Blocked => "Blocked",
                ProjectStatus.Cancelled => "Cancelled",
                ProjectStatus.Done => "Done",
                ProjectStatus.InProgress => "In Progress",
                ProjectStatus.Unknown => "NA",
                _ => throw new ArgumentException($"Unsupported status [{status}]"),
            };
        }
    }
}
