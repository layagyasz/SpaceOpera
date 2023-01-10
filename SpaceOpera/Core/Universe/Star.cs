using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class Star
    {
        private static readonly float BOLTZMAN_CONSTANT = 5.6704e-8f;

        public string Name { get; private set; }
        public float Temperature { get; }
        public float Radius { get; }
        public float Mass { get; }
        public float Luminosity { get; }

        public Star(float Temperature, float Radius, float Mass)
        {
            this.Temperature = Temperature;
            this.Radius = Radius;
            this.Mass = Mass;
            this.Luminosity = 
                (float)(BOLTZMAN_CONSTANT * 4 * Math.PI * Math.Pow(Radius, 2) * Math.Pow(Temperature, 4));
        }

        public void SetName(string Name)
        {
            this.Name = Name;
        }

        public override string ToString()
        {
            return string.Format(
                "[Star: Name={0}, Temperature={1}, Radius={2}, Mass={3}, Luminosity={4}",
                Name, 
                Temperature,
                Radius,
                Mass, 
                Luminosity);
        }
    }
}