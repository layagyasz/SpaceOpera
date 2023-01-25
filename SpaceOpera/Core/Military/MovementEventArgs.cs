using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public class MovementEventArgs : EventArgs
    {
        public INavigable? Origin { get; }
        public INavigable? Destination { get; }

        private MovementEventArgs(INavigable? origin, INavigable? destination)
        {
            Origin = origin;
            Destination = destination;
        }

        public static MovementEventArgs Create(INavigable? origin, INavigable? destination)
        {
            return new MovementEventArgs(origin, destination);
        }
    }
}