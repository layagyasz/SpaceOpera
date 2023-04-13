namespace SpaceOpera.Core.Military
{
    public interface IFormationDriver
    {
        EventHandler<MovementEventArgs>? Moved { get; set; }
        EventHandler<EventArgs>? OrderUpdated { get; set; }

        IFormation Formation { get; }

        void Tick(SpaceOperaContext context);
    }
}
