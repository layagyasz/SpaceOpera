using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe.Generator
{
    class StarGenerator : IKeyed
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public Sampler TemperatureSampler { get; set; }
        public Sampler RadiusSampler { get; set; }
        public Sampler MassSampler { get; set; }

        public Star Generate(Random Random)
        {
            return new Star(
                (float)TemperatureSampler.Sample(Random), 
                Constants.SOLAR_RADIUS * (float)RadiusSampler.Sample(Random),
                Constants.SOLAR_MASS * (float)MassSampler.Sample(Random));
        }
    }
}