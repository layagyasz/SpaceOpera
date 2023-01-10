using Cardamom.Graphing;
using DelaunayTriangulator;
using SFML.System;
using SFML.Window;
using SpaceOpera.Core.Voronoi;
using SpaceOpera.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SpaceOpera.Core.Universe.Generator
{
    class GalaxyGenerator
    {
        public float Radius { get; set; }
        public uint Arms { get; set; }
        public float Rotation { get; set; }
        public float CoreRadius { get; set; }
        public int StarCount { get; set; }
        public float TransitDensity { get; set; }
        public StarSystemGenerator StarSystemGenerator { get; set; }

        class SystemWrapper : Pathable<SystemWrapper>
        {
            public StarSystem System { get; }
            public bool Passable { get; } = true;
            private HashSet<SystemWrapper> _NeighborWrappers { get; } = new HashSet<SystemWrapper>();

            public SystemWrapper(StarSystem System)
            {
                this.System = System;
            }

            public double DistanceTo(SystemWrapper Node)
            {
                return HeuristicDistanceTo(Node);
            }

            public double HeuristicDistanceTo(SystemWrapper Node)
            {
                return MathUtils.Distance(System.Position, Node.System.Position);
            }

            public void SetNeighbors(IEnumerable<SystemWrapper> Wrappers)
            {
                foreach (var wrapper in Wrappers)
                {
                    _NeighborWrappers.Add(wrapper);
                }
                System.SetNeighbors(Wrappers.Select(x => x.System));
            }

            public IEnumerable<SystemWrapper> Neighbors()
            {
                return _NeighborWrappers;
            }
        }

        public Galaxy Generate(Random Random)
        {
            List<Vertex> vertices = new List<Vertex>();
            int starId = 0;
            while (starId<StarCount)
            {
                double x = Random.NextDouble() * 2 - 1;
                double y = Random.NextDouble() * 2 - 1;
                double angle = Math.Atan2(y, x);
                float radius = (float)Math.Sqrt(x * x + y * y);
                if (Random.NextDouble() < StarDensityFn(Arms, Rotation, angle, radius))
                {
                    ++starId;
                    float modifiedRadius = radius * (Radius - CoreRadius) + CoreRadius;
                    vertices.Add(
                        new Vertex(modifiedRadius * (float)Math.Cos(angle), modifiedRadius * (float)Math.Sin(angle)));
                }
            }

            Triangulator triangulator = new Triangulator();
            List<Triad> triads = triangulator.Triangulation(vertices);
            VoronoiGrapher.NeighborsResult result = VoronoiGrapher.GetNeighbors(vertices, triads);

            List<SystemWrapper> systemWrappers = new List<SystemWrapper>();
            for (int i=0; i< StarCount; ++i)
            {
                systemWrappers.Add(
                    new SystemWrapper(
                        StarSystemGenerator.Generate(Random, new Vector2f(vertices[i].x, vertices[i].y))));
            }
            for (int i=0; i < StarCount;++i)
            {
                var neighbors = result.Neighbors[i].Where(x => x >= 0).Select(x => systemWrappers[x]).ToList();
                systemWrappers[i].SetNeighbors(neighbors);
            }
            var spanningTree = 
                new MinimalSpanning<SystemWrapper>(systemWrappers, x => x.Neighbors(), (x, y) => x.DistanceTo(y));
            foreach (var transit in spanningTree.GetEdges())
            {
                transit.Item1.System.AddTransit(transit.Item2.System);
                transit.Item2.System.AddTransit(transit.Item1.System);
            }
            var distances = systemWrappers.SelectMany(x => x.Neighbors().Select(y => (float)x.DistanceTo(y))).ToList();
            var mean = distances.Average();
            var stdDev = MathUtils.StandardDeviation(distances, mean);
            var closed = new HashSet<SystemWrapper>();
            foreach (var systemWrapper in systemWrappers)
            {
                closed.Add(systemWrapper);
                foreach (var neighbor in systemWrapper.Neighbors())
                {
                    if (closed.Contains(neighbor) 
                        || systemWrapper.System.Transits.Values.Any(x => x.TransitSystem == neighbor.System))
                    {
                        continue;
                    }
                    if (Random.NextDouble() 
                        < TransitDensityFn(TransitDensity, (systemWrapper.DistanceTo(neighbor) - mean) / stdDev)) {
                        systemWrapper.System.AddTransit(neighbor.System);
                        neighbor.System.AddTransit(systemWrapper.System);
                    }
                }
            }

            return new Galaxy(Radius, systemWrappers.Select(x => x.System));
        }

        private static double StarDensityFn(uint Arms, float Rotation, double Angle, double Radius)
        {
            if (Radius > 1)
            {
                return 0;
            }
            return .5 * Math.Max(
                (Math.Cos(Angle * Arms + Radius * Rotation) + 1) * Math.Sqrt(1 - Radius), 1 - Radius);
        }

        private static double TransitDensityFn(float Density, double StandardDeviations)
        {
            return 2 * Density / (1 + Math.Exp(20 * StandardDeviations));
        }
    }
}