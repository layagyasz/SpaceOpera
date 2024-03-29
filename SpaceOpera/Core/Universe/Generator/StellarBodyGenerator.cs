using Cardamom;
using Cardamom.Collections;
using Cardamom.Graphing;
using Cardamom.Mathematics.Coordinates;
using Cardamom.Mathematics.Coordinates.Projections;
using Cardamom.Utils.Generators.Samplers;
using DelaunayTriangulator;
using OpenTK.Mathematics;

namespace SpaceOpera.Core.Universe.Generator
{
    public class StellarBodyGenerator: IKeyed
    {
        class RegionWrapper : SeededGraphPartition.ISeed<SubRegionWrapper>
        {
            public SubRegionWrapper Origin { get; }
            public float InitialCost => 0;
            public HashSet<SubRegionWrapper>? Children { get; set; }

            public RegionWrapper(SubRegionWrapper origin)
            {
                Origin = origin;
            }

            public float GetPenalty(SubRegionWrapper wrapper)
            {
                return 0;
            }
        }

        class SubRegionWrapper : IGraphNode
        {
            public StellarBodySubRegion Region { get; }
            public readonly List<IGraphEdge> _edges = new();

            public SubRegionWrapper(StellarBodySubRegion region)
            {
                Region = region;
            }

            public void AddNeighbors(IEnumerable<SubRegionWrapper> neighbors)
            {
                _edges.AddRange(neighbors.Select(x => new DefaultGraphEdge(this, x, DistanceTo(x))));
            }

            public IEnumerable<IGraphEdge> GetEdges()
            {
                return _edges;
            }

            private float DistanceTo(SubRegionWrapper otherRegion)
            {
                int biomeCost = Region.Biome == otherRegion.Region.Biome ? 0 : 4;
                return 1 + biomeCost;
            }
        }

        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ISampler? RadiusSampler { get; set; }
        public float Density { get; set; }
        public float RegionDensity { get; set; }
        public float SubRegionDensity { get; set; }
        public uint StructureNodeDensity { get; set; }
        public StellarBodySurfaceGenerator? SurfaceGenerator { get; set; }
        public AtmosphereGenerator? AtmosphereGenerator { get; set; }

        public StellarBody Generate(Orbit orbit, GeneratorContext context)
        {
            while (true)
            {
                try
                {
                    return GenerateAux(orbit, context);
                }
                catch (Exception e)
                {
                    context.Logger!.ForType(typeof(StellarBodyGenerator)).AtWarning().Log(e.ToString());
                    continue;
                }
            }
        }

        private StellarBody GenerateAux(Orbit orbit, GeneratorContext context)
        {
            var random = context.Random;
            float radius = RadiusSampler!.Generate(random);
            int subRegionCount =
                Math.Min(
                    context.StellarBodySurfaceGeneratorResources!.Resolution 
                        * context.StellarBodySurfaceGeneratorResources.Resolution, 
                    (int)Math.Ceiling(4 * SubRegionDensity * Math.PI * radius * radius));

            Vector3[] centers = new Vector3[subRegionCount];
            List<Vertex> vertices = new(subRegionCount - 1);
            var projection = new StereographicProjection.Cartesian();
            for (int i = 0; i < subRegionCount - 1; ++i)
            {
                float z = 2 * random.NextSingle() - 1;
                float r = MathF.Sqrt(1 - z * z);
                float theta = MathF.Tau * random.NextSingle();
                var point = new Vector3(r * MathF.Cos(theta), r * MathF.Sin(theta), z);
                centers[i] = point;
                var projected = projection.Project(point);
                vertices.Add(new Vertex(projected.X, projected.Y));
            }

            List<Triad> triads = VoronoiGrapher.GetTriangulation(vertices);
            VoronoiGrapher.VoronoiNeighborsResult result = VoronoiGrapher.GetNeighbors(vertices, triads);
            result.Neighbors.Add(result.EdgeIndices);
            centers[subRegionCount - 1] = new(0, 1, 0);

            var temp = orbit.GetStellarTemperature();
            var parameters = SurfaceGenerator!.Generate(random);
            Biome[] biomes = 
                SurfaceGenerator!.Get(temp, parameters, centers, context.StellarBodySurfaceGeneratorResources);
            List<SubRegionWrapper> subRegionWrappers = new();
            for (int i=0;i<subRegionCount;++i)
            {
                var subRegion = new StellarBodySubRegion(i, centers[i], biomes[i]);
                subRegionWrappers.Add(new SubRegionWrapper(subRegion));
            }
            
            for(int i=0; i<subRegionCount;++i)
            {
                subRegionWrappers[i].Region.SetNeighbors(
                    result.Neighbors[i]
                    .Select(x => subRegionWrappers[x >= 0 ? x : subRegionCount - 1].Region));
                subRegionWrappers[i].AddNeighbors(
                    result.Neighbors[i].Select(x => subRegionWrappers[x >= 0 ? x : subRegionCount - 1]));
            }

            int regionCount = (int)Math.Ceiling(4 * RegionDensity * Math.PI * radius * radius);
            List<RegionWrapper> regionWrappers = new();
            List<int> cores = new();
            for (int i=0; i <regionCount; ++i)
            {
                int core;
                do
                {
                    core = random.Next(0, subRegionCount);
                }
                while (cores.Contains(core) || subRegionWrappers[core]._edges.Count < 4);
                cores.Add(core);
                var region = new RegionWrapper(subRegionWrappers[core]);
                regionWrappers.Add(region);
            }

            bool homogenous = SurfaceGenerator.IsHomogenous(temp);
            foreach (var partition in 
                SeededGraphPartition.Compute<RegionWrapper, SubRegionWrapper>(regionWrappers, x => true))
            {
                var region = partition.Seed;
                if (homogenous)
                {
                    region.Children = new(partition.Nodes);
                }
                else
                {
                    var partitioned =
                        partition.Nodes
                            .Where(x => x.Region.Biome.IsTraversable != region.Origin.Region.Biome.IsTraversable)
                            .ToHashSet();
                    region.Children = new(partition.Nodes);
                    if (partitioned.Count > 0)
                    {
                        region.Children.RemoveWhere(partitioned.Contains);
                        var newWrapper =
                            new RegionWrapper(
                                partitioned.ArgMax(
                                    x => -MathUtils.ArcLength(region.Origin.Region.Center, x.Region.Center, radius))!)
                            {
                                Children = partitioned
                            };
                        regionWrappers.Add(newWrapper);
                    }
                }
            }
            if (regionWrappers.Count > regionCount)
            {
                regionWrappers.Sort(
                    Comparer<RegionWrapper>.Create((x, y) => x.Children!.Count.CompareTo(y.Children!.Count)));
                var regionsToRemove = regionWrappers.Count - regionCount;
                int limit = 0;
                while (regionsToRemove > 0 && limit < regionWrappers.Count)
                {
                    var region = regionWrappers[0];
                    var regionToCombine =
                        regionWrappers.FirstOrDefault(
                            x => x != region 
                                && region.Origin.Region.Biome.IsTraversable == x.Origin.Region.Biome.IsTraversable 
                                && x.Children!.SelectMany(y => y.GetEdges())
                                         .Select(x => x.End).Any(region.Children!.Contains));
                    if (regionToCombine != null)
                    {
                        foreach (var child in region.Children!)
                        {
                            regionToCombine.Children!.Add(child);
                        }
                        region.Children!.Clear();
                        --regionsToRemove;
                    }
                    else
                    {
                        regionWrappers.Add(region);
                    }
                    ++limit;
                    regionWrappers.RemoveAt(0);
                }
            }

            List<StellarBodyRegion> regions = new();
            for (int i = 0; i < regionWrappers.Count; ++i)
            {
                var region = 
                    new StellarBodyRegion(
                        regionWrappers[i].Origin.Region, regionWrappers[i].Children!.Select(x => x.Region));
                region.AddStructureNodes((uint)(StructureNodeDensity * regionWrappers[i].Children!.Count));
                regions.Add(region);
                foreach (var subRegion in regionWrappers[i].Children!)
                {
                    subRegion.Region.SetParentRegion(region);
                }
            }

            int atmosphericRegionCount = (int)(2 * Math.PI * radius * AtmosphereGenerator!.RegionDensity) + 1;
            float atmosphereArc = MathF.Tau / atmosphericRegionCount;
            List<List<StellarBodySubRegion>> atmosphereRegionMembers = new();
            for (int i=0; i<atmosphericRegionCount; ++i)
            {
                atmosphereRegionMembers.Add(new List<StellarBodySubRegion>());
            }
            foreach (var subRegion in subRegionWrappers)
            {
                double angle = 
                    (subRegion.Region.Center.AsSpherical().Azimuth + 2 * Math.PI) % (2 * Math.PI) / (2 * Math.PI);
                int atmosphereRegion = (int)(angle * atmosphericRegionCount);
                atmosphereRegionMembers[atmosphereRegion].Add(subRegion.Region);
            }

            var orbitRegions = new List<StationaryOrbitRegion>();
            for (int i=0; i< atmosphericRegionCount; ++i)
            {
                orbitRegions.Add(new($"{(char)(i + 65)}", (i + 0.5f) * atmosphereArc, atmosphereRegionMembers[i]));
            }

            float mass = 4000000000 * Density * MathF.PI * radius * radius * radius / 3;
            return new StellarBody(
                Key,
                parameters,
                radius,
                mass,
                orbit,
                AtmosphereGenerator.Generate(mass, radius, context), 
                regions, 
                orbitRegions);
        }
    }
}