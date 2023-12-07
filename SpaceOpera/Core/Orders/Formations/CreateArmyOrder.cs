using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Orders.Formations
{
    public class CreateArmyOrder : IOrder
    {
        public Faction Faction { get; }

        public CreateArmyOrder(Faction faction)
        {
            Faction = faction;
        }

        public bool Execute(World world)
        {
            var army = new Army(Faction);
            army.SetName(Faction.NameGenerator.GenerateNameForArmy(world.Random));
            world.Formations.AddArmy(army);
            return true;
        }

        public ValidationFailureReason Validate(World world)
        {
            return ValidationFailureReason.None;
        }
    }
}
