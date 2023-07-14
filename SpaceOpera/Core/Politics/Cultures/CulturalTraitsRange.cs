using Cardamom.Mathematics;

namespace SpaceOpera.Core.Politics.Cultures
{
    public struct CulturalTraitsRange
    {
        public IntInterval AuthoritarianEgalitarian { get; set; } = new(-2, 2);
        public IntInterval IndividualistCollectivist { get; set; } = new(-2, 2);
        public IntInterval AggressivePassive { get; set; } = new(-2, 2);
        public IntInterval ConventionalDynamic { get; set; } = new(-2, 2);
        public IntInterval MonumentalHumble { get; set; } = new(-2, 2);
        public IntInterval IndulgentAustere { get; set; } = new(-2, 2);

        public CulturalTraitsRange() { }

        public bool Contains(CulturalTraits traits)
        {
            return AuthoritarianEgalitarian.Contains(traits.AuthoritarianEgalitarian)
                && IndividualistCollectivist.Contains(traits.IndividualistCollectivist)
                && AggressivePassive.Contains(traits.AggressivePassive)
                && ConventionalDynamic.Contains(traits.ConventionalDynamic)
                && MonumentalHumble.Contains(traits.MonumentalHumble)
                && IndulgentAustere.Contains(traits.IndulgentAustere);
        }
    }
}
