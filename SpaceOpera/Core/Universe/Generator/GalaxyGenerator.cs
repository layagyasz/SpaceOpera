using Cardamom.Graphing;
using Cardamom.Utils.Generators.Samplers;
using DelaunayTriangulator;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    public class GalaxyGenerator
    {
        private static readonly ISampler s_YSampler = new NormalSampler(0, 100);

        public struct Parameters
        {
            public float Radius { get; set; }
            public int Arms { get; set; }
            public float Rotation { get; set; }
            public float StarDensity { get; set; }
            public float TransitDensity { get; set; }
        }

        public StarSystemGenerator? StarSystemGenerator { get; set; }

        class SystemWrapper : IGraphNode
        {
            public StarSystem System { get; }
            private readonly List<IGraphEdge> _edges = new();

            public SystemWrapper(StarSystem system)
            {
                System = system;
            }

            public IEnumerable<IGraphEdge> GetEdges()
            {
                return _edges;
            }

            public void SetNeighbors(IEnumerable<SystemWrapper> wrappers)
            {
                _edges.AddRange(
                    wrappers.Select(
                        x => new DefaultGraphEdge(this, x, Vector3.Distance(System.Position, x.System.Position))));
                System.SetNeighbors(wrappers.Select(x => x.System));
            }
        }

        public Galaxy Generate(Parameters parameters, GeneratorContext context)
        {
            var random = context.Random;
            List<Vertex> vertices = new();
            float coreRadius = 0;
            int starCount = 
                (int)(parameters.StarDensity * MathF.PI * 
                (parameters.Radius * parameters.Radius - coreRadius * coreRadius));
            int starId = 0;
            while (starId < starCount)
            {
                float x = random.NextSingle() * 2 - 1;
                float y = random.NextSingle() * 2 - 1;
                float angle = MathF.Atan2(y, x);
                float radius = MathF.Sqrt(x * x + y * y);
                if (random.NextSingle() < StarDensityFn(parameters.Arms, parameters.Rotation, angle, radius))
                {
                    ++starId;
                    float modifiedRadius = radius * (parameters.Radius - coreRadius) + coreRadius;
                    vertices.Add(
                        new Vertex(modifiedRadius * MathF.Cos(angle), modifiedRadius * MathF.Sin(angle)));
                }
            }

            Triangulator triangulator = new();
            List<Triad> triads = triangulator.Triangulation(vertices);
            VoronoiGrapher.VoronoiNeighborsResult result = VoronoiGrapher.GetNeighbors(vertices, triads);

            List<SystemWrapper> systemWrappers = new();
            for (int i=0; i < starCount; ++i)
            {
                context.Logger!.ForType(typeof(GalaxyGenerator)).AtInfo().EverySeconds(5).Log($"\tCreated {i} systems");
                systemWrappers.Add(
                    new SystemWrapper(
                        StarSystemGenerator!.Generate(
                            new Vector3(vertices[i].x, s_YSampler.Generate(random), vertices[i].y), context)));
            }
            for (int i=0; i < starCount; ++i)
            {
                var neighbors = result.Neighbors[i].Where(x => x >= 0).Select(x => systemWrappers[x]).ToList();
                systemWrappers[i].SetNeighbors(neighbors);
            }
            foreach (var transit in MinimalSpanningTree.Compute(systemWrappers))
            {
                var start = (SystemWrapper)transit.Start;
                var end = (SystemWrapper)transit.End;
                start.System.AddTransit(end.System);
                end.System.AddTransit(start.System);
            }
            var distances = systemWrappers.SelectMany(x => x.GetEdges().Select(y => y.Cost)).ToList();
            var mean = distances.Average();
            var stdDev = MathUtils.StandardDeviation(distances, mean);
            var closed = new HashSet<SystemWrapper>();
            foreach (var systemWrapper in systemWrappers)
            {
                closed.Add(systemWrapper);
                foreach (var edge in systemWrapper.GetEdges())
                {
                    var neighbor = (SystemWrapper)edge.End;
                    if (closed.Contains(neighbor) 
                        || systemWrapper.System.Transits.Values.Any(x => x.TransitSystem == neighbor.System))
                    {
                        continue;
                    }
                    if (random.NextSingle() 
                        < TransitDensityFn(parameters.TransitDensity, (edge.Cost - mean) / stdDev)) {
                        systemWrapper.System.AddTransit(neighbor.System);
                        neighbor.System.AddTransit(systemWrapper.System);
                    }
                }
            }

            return new Galaxy(parameters.Radius, systemWrappers.Select(x => x.System));
        }

        private static float StarDensityFn(int arms, float rotation, float angle, float radius)
        { 
            if (radius > 1)
            {
                return 0;
            }
            return Math.Max(
                (0.5f * MathF.Cos(angle * arms + radius * rotation) + 1) * MathF.Sqrt(1 - radius), 1 - radius);
        }

        private static float TransitDensityFn(float density, float standardDeviations)
        {
            return 2 * density / (1 + MathF.Exp(20 * standardDeviations));
        }
    }
}