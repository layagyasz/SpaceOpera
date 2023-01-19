namespace SpaceOpera.Core.Universe
{
    public class SolarOrbitRegion : INavigable
    {
        public string Name => string.Format("{0} Orbit", LocalOrbit.StellarBody.Name);
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Space;
        public LocalOrbitRegion LocalOrbit { get; }

        public SolarOrbitRegion(LocalOrbitRegion localOrbit)
        {
            this.LocalOrbit = localOrbit;
        }
    }
}