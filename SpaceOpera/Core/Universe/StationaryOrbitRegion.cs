namespace SpaceOpera.Core.Universe
{
    public class StationaryOrbitRegion : INavigable
    {
        public string Name { get; }
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public float Theta { get; }
        public List<StellarBodySubRegion> SubRegions { get; }

        public StationaryOrbitRegion(string name, float theta, IEnumerable<StellarBodySubRegion> subRegions)
        {
            Name = name;
            Theta = theta;
            SubRegions = subRegions.ToList();
        }
    }
}