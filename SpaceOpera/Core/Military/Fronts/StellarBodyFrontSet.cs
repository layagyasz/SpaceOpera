using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Fronts
{
    public class StellarBodyFrontSet
    {
        public EventHandler<EventArgs>? Changed { get; set; }

        public StellarBody StellarBody { get; }

        private List<Front> _fronts = new();

        public StellarBodyFrontSet(StellarBody stellarBody)
        {
            StellarBody = stellarBody;
            StellarBody.OccupationChanged += HandleChange;
        }

        public void Update()
        {
            // Implement computing fronts.
        }

        private void HandleChange(object? sender, EventArgs e)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
