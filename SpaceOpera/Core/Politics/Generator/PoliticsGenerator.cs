using Cardamom.Collections;
using Cardamom.Trackers;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Politics.Generator
{
    public class PoliticsGenerator
    {
        class FactionWrapper : DijkstraRegion<RegionWrapper>
        {
            public Faction Faction { get; }
            public RegionWrapper Center { get; }
            public double StartDistance => 0;

            public HashSet<RegionWrapper> Regions { get; } = new();

            public FactionWrapper(Faction faction, RegionWrapper center)
            {
                Faction = faction;
                Center = center;
            }

            public void Add(RegionWrapper child)
            {
                Regions.Add(child);
            }
        }

        class RegionWrapper : Pathable<RegionWrapper>
        {
            public bool Passable => true;
            public StellarBody StellarBody { get; }
            public StellarBodyRegion Region { get; }
            public List<RegionEdgeWrapper> Edges { get; } = new();
            public MultiQuantity<Culture> CultureWeights { get; } = new();

            public RegionWrapper(StellarBody stellarBody, StellarBodyRegion region)
            {
                StellarBody = stellarBody;
                Region = region;
            }

            public IEnumerable<RegionWrapper> Neighbors()
            {
                return Edges.Select(x => x.End);
            }

            public double DistanceTo(RegionWrapper neighbor)
            {
                return Edges.First(x => x.End == neighbor).Distance;
            }

            public double HeuristicDistanceTo(RegionWrapper other)
            {
                return DistanceTo(other);
            }
        }

        class RegionEdgeWrapper
        {
            public RegionWrapper End { get; }
            public float Distance { get; }
            public float Falloff { get; }

            public RegionEdgeWrapper(RegionWrapper end, float distance, float falloff)
            {
                End = end;
                Distance = distance;
                Falloff = falloff;
            }
        }

        public CultureGenerator? Culture { get; set; }
        public BannerGenerator? Banner { get; set; }
        public FactionGenerator? Faction { get; set; }
        public DesignGenerator? Design { get; set; }
        public FleetGenerator? Fleet { get; set; }
        public float BaseLinkChance { get; set; }
        public uint Cultures { get; set; }
        public uint States { get; set; }

        public void Generate(World world, Culture playerCulture, Faction playerFaction, Random random)
        {
            var nodes = new Dictionary<StellarBodyRegion, RegionWrapper>();
            var homeRegions = new WeightedVector<RegionWrapper>();
            foreach (var system in world.Galaxy.Systems)
            {
                var regionOptions = new WeightedVector<RegionWrapper>();
                foreach (var body in system.Orbiters.Concat(
                    system.Transits.Values.SelectMany(x => x.TransitSystem.Orbiters)))
                {
                    foreach (var region in body.Regions)
                    {
                        if (region.DominantBiome.IsTraversable)
                        {
                            var node = GetOrCreateNode(nodes, body, region);
                            regionOptions.Add(GetLinkChance(region), node);
                        }
                    }
                }

                for (int i=0; i < system.Orbiters.Count; ++i)
                {
                    var body = system.Orbiters[i];
                    foreach (var region in body.Regions)
                    {
                        var node = GetOrCreateNode(nodes, body, region);
                        if (region.DominantBiome.IsTraversable)
                        {
                            homeRegions.Add(node, GetHabitability(region));
                        }
                        foreach (var neighbor in region.GetNeighbors())
                        {
                            var neighborNode = GetOrCreateNode(nodes, body, neighbor);
                            float distance = GetDistance(region, neighbor);
                            float falloff = GetFalloff(region, neighbor);
                            node.Edges.Add(new RegionEdgeWrapper(neighborNode, distance, falloff));
                            neighborNode.Edges.Add(new RegionEdgeWrapper(node, distance, falloff));
                        }
                        if (region.DominantBiome.IsTraversable && random.NextDouble() < GetLinkChance(region))
                        {
                            var jump = regionOptions.Get(random.NextSingle());
                            float distance = GetDistance(region, jump.Region);
                            float falloff = GetFalloff(region, jump.Region);
                            node.Edges.Add(new RegionEdgeWrapper(jump, distance, falloff));
                            jump.Edges.Add(new RegionEdgeWrapper(node, distance, falloff));
                        }
                    }

                    if (i < system.Orbiters.Count - 1)
                    {
                        AddRandomEdge(body, system.Orbiters[i + 1], nodes, random);
                    }
                    else
                    {
                        foreach (var neighbor in system.Transits.Values.Select(x => x.TransitSystem))
                        {
                            if (neighbor.Orbiters.Count > 0)
                            {
                                AddRandomEdge(body, neighbor.Orbiters.Last(), nodes, random);
                            }
                        }
                    }
                }
            }

            var cultures = new List<Culture>();
            var playerHomeRegion = homeRegions.Get(random.Next(0, homeRegions.Count));
            PlaceCulture(playerCulture, playerHomeRegion);
            var chosenHomeRegions = new List<RegionWrapper>();
            for (int i = 0; i < Cultures; ++i)
            {
                var culture = Culture!.Generate(random);
                cultures.Add(culture);
                var homeRegion = homeRegions.Get(random.NextSingle());
                chosenHomeRegions.Add(homeRegion);
                PlaceCulture(culture, homeRegion);
            }
            world.AddAllCultures(cultures);

            var states = new List<FactionWrapper>();
            var playerStateWrapper = new FactionWrapper(playerFaction, playerHomeRegion);
            states.Add(playerStateWrapper);
            var closedRegions = new HashSet<RegionWrapper>
            {
                playerHomeRegion
            };
            var statePool = new DijkstraPool<RegionWrapper>();
            statePool.Drop(playerStateWrapper);

            var banners = Banner!.GenerateUnique(States, random).ToList();
            for (int i = 0; i < States; ++i)
            {
                var homeRegion = 
                    i < chosenHomeRegions.Count ? chosenHomeRegions[i] : homeRegions.Get(random.NextSingle());
                while (closedRegions.Contains(homeRegion) || homeRegion.CultureWeights.Count == 0)
                {
                    homeRegion = homeRegions.Get(random.NextSingle());
                }
                closedRegions.Add(homeRegion);
                var stateWrapper =
                    new FactionWrapper(
                        Faction!.Generate(
                            homeRegion.CultureWeights.ArgMax(x => x.Value).Key, banners[i], random), homeRegion);
                states.Add(stateWrapper);
                statePool.Drop(stateWrapper);
            }
            world.AddAllFactions(states.Select(x => x.Faction));
            statePool.Resolve();

            foreach (var state in states)
            {
                Design!.Generate(world, state.Faction, random);
                foreach (var region in state.Regions)
                {
                    region.Region.SetName(state.Faction.NameGenerator.GenerateNameFor(region.Region, random));
                    region.Region.SetSovereign(state.Faction);
                }
                var hq = 
                    world.Galaxy.Systems
                        .SelectMany(x => x.Orbiters)
                        .SelectMany(x => x.OrbitRegions)
                        .Where(x => x.SubRegions.Contains(state.Regions.First().Region.Center))
                        .First();
                Fleet!.Generate(world, state.Faction, hq, random);
            }

            foreach (var system in world.Galaxy.Systems)
            {
                var dominantFaction = 
                    system.Orbiters.SelectMany(x => x.Regions).GroupBy(x => x.Sovereign).ArgMax(x => x.Count())?.Key;
                if (dominantFaction != null)
                {
                    system.Star.SetName(dominantFaction.NameGenerator.GenerateNameForStar(random));
                    system.SetName(dominantFaction.NameGenerator.GenerateNameFor(system, random));

                    foreach (var stellarBody in system.Orbiters)
                    {
                        var stellarBodyDominantFaction = 
                            stellarBody.Regions.GroupBy(x => x.Sovereign).ArgMax(x => x.Count())?.Key 
                            ?? dominantFaction;
                        stellarBody.SetName(
                            stellarBodyDominantFaction.NameGenerator.GenerateNameFor(stellarBody, system, random));
                    }
                }
            }
        }

        private float GetLinkChance(StellarBodyRegion region)
        {
            float chance = BaseLinkChance;
            if (!region.DominantBiome.IsHabitable)
            {
                chance *= 0.1f;
            }
            return chance;
        }

        private static void AddRandomEdge(
            StellarBody left, StellarBody right, Dictionary<StellarBodyRegion, RegionWrapper> wrappers, Random random)
        {
            var region = left.Regions[random.Next(0, left.Regions.Count)];
            var jump = right.Regions[random.Next(0, right.Regions.Count)];

            var node = GetOrCreateNode(wrappers, left, region);
            var jumpNode = GetOrCreateNode(wrappers, right, jump);

            float distance = GetDistance(region, jump);
            float falloff = GetFalloff(region, jump);
            node.Edges.Add(new RegionEdgeWrapper(jumpNode, distance, falloff));
            jumpNode.Edges.Add(new RegionEdgeWrapper(node, distance, falloff));
        }

        private static void PlaceCulture(Culture culture, RegionWrapper homeRegion)
        {
            var queue = new Heap<RegionWrapper, float>();
            var closed = new HashSet<RegionWrapper>();
            queue.Push(homeRegion, 0);
            homeRegion.CultureWeights.Add(culture, 1);
            while (queue.Count > 0)
            {
                var current = queue.Pop();
                closed.Add(current);
                foreach (var edge in current.Edges)
                {
                    if (!closed.Contains(edge.End))
                    {
                        var weight = current.CultureWeights.Get(culture) * edge.Falloff;
                        if (edge.End.CultureWeights.Get(culture) < weight)
                        {
                            edge.End.CultureWeights[culture] = weight;
                            queue.Remove(edge.End);
                            queue.Push(edge.End, weight);
                        }
                    }
                }
            }
        }

        private static float GetHabitability(StellarBodyRegion region)
        {
            if (region.DominantBiome.IsHabitable)
            {
                return 1;
            }
            else return 0.01f;
        }

        private static float GetFalloff(StellarBodyRegion left, StellarBodyRegion right)
        {
            return 0.9f;
        }

        private static float GetDistance(StellarBodyRegion left, StellarBodyRegion right)
        {
            return 1;
        }

        private static RegionWrapper GetOrCreateNode(
            Dictionary<StellarBodyRegion, RegionWrapper> wrappers, StellarBody stellarBody, StellarBodyRegion region)
        {
            if (wrappers.TryGetValue(region, out var node))
            {
                return node;
            }
            else
            {
                node = new RegionWrapper(stellarBody, region);
                wrappers.Add(region, node);
                return node;
            }
        }
    }
}