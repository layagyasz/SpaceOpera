using SpaceOpera.Core.Military.Ai;

namespace SpaceOpera.Core.Military
{
    public interface IFormationDriver
    {
        EventHandler<EventArgs>? OrderUpdated { get; set; }
        IFormation Formation { get; }
        void Tick(SpaceOperaContext context);
    }
}
