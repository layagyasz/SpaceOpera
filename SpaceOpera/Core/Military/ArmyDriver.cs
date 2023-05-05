using SpaceOpera.Core.Military.Ai;

namespace SpaceOpera.Core.Military
{
    public class ArmyDriver : IFormationDriver
    {
        public EventHandler<EventArgs>? OrderUpdated { get; set; }
        public IFormation Formation => Army;
        public Army Army { get; }

        public ArmyDriver(Army army)
        {
            Army = army;
        }

        public void Tick(SpaceOperaContext context) { }
    }
}
