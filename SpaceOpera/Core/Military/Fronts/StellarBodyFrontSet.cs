using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Fronts
{
    public class StellarBodyFrontSet
    {
        private readonly record struct OccupationKey(Faction? Faction, bool IsTraversable);

        public EventHandler<EventArgs>? Changed { get; set; }

        public StellarBody StellarBody { get; }

        private List<Front> _fronts = new();

        public StellarBodyFrontSet(StellarBody stellarBody)
        {
            StellarBody = stellarBody;
            StellarBody.OccupationChanged += HandleChange;
        }

        public IEnumerable<Front> GetFronts()
        {
            return _fronts;
        }

        public void Update()
        {
            var fronts = new List<Front>();
            var visited = new HashSet<FrontEdge>();
            foreach (var subRegion in StellarBody.Regions.SelectMany(x => x.SubRegions))
            {
                var key = GetKey(subRegion);
                if (!key.IsTraversable || key.Faction == null)
                {
                    continue;
                }
                for (int i = 0; i < subRegion.Neighbors!.Length; ++i)
                {
                    var edge = new FrontEdge(subRegion, i);
                    var edgeKey = GetKey(edge);
                    if (edgeKey != key && !visited.Contains(edge))
                    {
                        fronts.AddRange(TraceFrom(edge, visited));
                    }
                }
            }
            _fronts = fronts;
        }

        private void HandleChange(object? sender, EventArgs e)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private static OccupationKey GetKey(FrontEdge edge)
        {
            return GetKey(edge.Region.Neighbors![edge.Index]);
        }

        private static OccupationKey GetKey(StellarBodySubRegion subRegion)
        {
            return new(subRegion.Occupation ?? subRegion.ParentRegion!.Sovereign, subRegion.Biome.IsTraversable);
        }

        private static List<Front> TraceFrom(FrontEdge start, ISet<FrontEdge> visited)
        {
            var seedKey = GetKey(start.Region);
            var result = new List<Front>();
            var startKey = GetKey(start);

            var currentFront = new List<FrontEdge>();
            var current = start;
            var currentKey = startKey;
            do
            {
                visited.Add(current);
                var key = GetKey(current);
                if (key != currentKey)
                {
                    result.Add(new(seedKey.Faction!, currentKey.Faction, currentFront));
                    currentFront = new();
                    currentKey = key;
                }
                currentFront.Add(current);
                current = Step(current);
            }
            while (!start.Equals(current));
            if (currentKey == startKey && result.Count > 1)
            {
                result[0] = new(seedKey.Faction!, startKey.Faction, currentFront.Concat(result[0].Edges).ToList());
            }
            else
            {
                result.Add(new(seedKey.Faction!, currentKey.Faction, currentFront));
            }
            return result;
        }

        private static FrontEdge Step(FrontEdge current)
        {
            int nextIndex;
            nextIndex = StepIndex(current.Index, current.Region.Neighbors!.Length);
            var edge = new FrontEdge(current.Region, nextIndex);
            if (GetKey(current.Region) != GetKey(edge))
            {
                return edge;
            }
            var neighbor = current.Region.Neighbors[nextIndex];
            return new(
                neighbor, StepIndex(Array.IndexOf(neighbor.Neighbors!, current.Region), neighbor.Neighbors!.Length));
        }

        private static int StepIndex(int index, int mod)
        {
            return (index + 1) % mod;
        }
    }
}
