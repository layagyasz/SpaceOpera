namespace SpaceOpera.Core.Military.Intelligence
{
    public class FormationIntelligence
    {
        class SingleFormationIntelligence
        {
            public double SpottingProgress { get; set; }
        }

        private readonly Dictionary<IAtomicFormation, SingleFormationIntelligence> _formationIntelligence = new();

        public bool IsSpotted(IAtomicFormation formation)
        {
            _formationIntelligence.TryGetValue(formation, out var intel);
            return intel != null && Math.Abs(intel.SpottingProgress - 1) < double.Epsilon;
        }

        public void Spot(IAtomicFormation formation, double progress)
        {
            _formationIntelligence.TryGetValue(formation, out var intel);
            if (intel == null)
            {
                intel = new SingleFormationIntelligence();
                _formationIntelligence.Add(formation, intel);
            }
            intel.SpottingProgress = Math.Min(1, intel.SpottingProgress + progress);
        }
    }
}