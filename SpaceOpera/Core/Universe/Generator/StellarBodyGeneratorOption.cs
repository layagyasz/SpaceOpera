using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe.Generator
{
    class StellarBodyGeneratorOption
    {
        public StellarBodyGenerator Generator { get; set; }
        public float Weight { get; set; }
        public Range ThermalRange { get; set; }
        public Range GravitationalRange { get; set; }

        public bool Satisfies(float Temperature, float Gravity)
        {
            return ThermalRange.Contains(Temperature) && GravitationalRange.Contains(Gravity);
        }
    }
}