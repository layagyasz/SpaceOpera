using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Fronts
{
    public class FrontManager
    {
        private readonly Dictionary<StellarBody, StellarBodyFrontSet> _stellarBodyFronts;
        private readonly HashSet<StellarBodyFrontSet> _dirty = new();

        private FrontManager(Dictionary<StellarBody, StellarBodyFrontSet> stellarBodyFronts)
        {
            _stellarBodyFronts = stellarBodyFronts;
            foreach (var fronts in stellarBodyFronts.Values)
            {
                fronts.Changed += HandleChange;
            }
        }

        public static FrontManager Create(Galaxy galaxy)
        {
            return new(
                galaxy.Systems.SelectMany(x => x.Orbiters).ToDictionary(x => x, x => new StellarBodyFrontSet(x)));
        }

        public void Tick()
        {
            foreach (var fronts in _dirty)
            {
                fronts.Update();
            }
            _dirty.Clear();
        }

        private void HandleChange(object? sender, EventArgs e)
        {
            _dirty.Add((StellarBodyFrontSet)sender!);
        }
    }
}
