﻿using Cardamom.Utils.Generators.Samplers;

namespace SpaceOpera.Core.Universe.Generator
{
    public class OrbitGenerator
    {
        public ISampler? EccentricitySampler { get; set; }

        public Orbit Generate(Star star, float semiMajorAxis, GeneratorContext context)
        {
            return new Orbit(
                star, 
                2 * semiMajorAxis, 
                Math.Max(0, EccentricitySampler!.Generate(context.Random)), context.Random.NextSingle());
        }
    }
}
