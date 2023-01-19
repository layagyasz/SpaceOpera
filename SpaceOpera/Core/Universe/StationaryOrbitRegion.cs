namespace SpaceOpera.Core.Universe
{
    public class StationaryOrbitRegion : INavigable
    {
        public string Name { get; }
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public List<StellarBodySubRegion> SubRegions { get; }

        public StationaryOrbitRegion(string name, IEnumerable<StellarBodySubRegion> subRegions)
        {
            Name = name;
            SubRegions = subRegions.ToList();
        }
    }
}