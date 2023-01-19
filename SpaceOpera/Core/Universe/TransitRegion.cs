namespace SpaceOpera.Core.Universe
{
    public class TransitRegion : INavigable
    {
        public string Name => string.Format("{0} Transit", TransitSystem.Name);
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public StarSystem TransitSystem { get; }

        public TransitRegion(StarSystem transitSystem)
        {
            TransitSystem = transitSystem;
        }
    }
}
