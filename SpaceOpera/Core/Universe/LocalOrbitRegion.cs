namespace SpaceOpera.Core.Universe
{
    public class LocalOrbitRegion : INavigable
    {
        public string Name => string.Format("Local {0} Orbit", StellarBody.Name);
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public StellarBody StellarBody { get; }

        public LocalOrbitRegion(StellarBody stellarBody)
        {
            StellarBody = stellarBody;
        }
    }
}