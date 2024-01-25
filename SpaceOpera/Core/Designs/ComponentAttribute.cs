namespace SpaceOpera.Core.Designs
{
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