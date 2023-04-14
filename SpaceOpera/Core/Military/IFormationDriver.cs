using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public interface IFormationDriver
    {
        EventHandler<MovementEventArgs>? Moved { get; set; }
        EventHandler<EventArgs>? OrderUpdated { get; set; }

        IFormation Formation { get; }

        ICollection<INavigable> GetActiveRegion();

        void Tick(SpaceOperaContext context);
    }
}
