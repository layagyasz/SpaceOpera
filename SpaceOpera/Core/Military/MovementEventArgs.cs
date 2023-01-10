using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class MovementEventArgs : EventArgs
    {
        public INavigable Origin { get; }
        public INavigable Destination { get; }

        private MovementEventArgs(INavigable Origin, INavigable Destination)
        {
            this.Origin = Origin;
            this.Destination = Destination;
        }

        public static MovementEventArgs Create(INavigable Origin, INavigable Destination)
        {
            return new MovementEventArgs(Origin, Destination);
        }
    }
}