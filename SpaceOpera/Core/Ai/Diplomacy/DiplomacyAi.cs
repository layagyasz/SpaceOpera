using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Cultures;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Core.Ai.Diplomacy
{
    public class DiplomacyAi
    {
        public Faction Faction { get; }

        public DiplomacyAi(Faction faction)
        {
            Faction = faction;
        }

        public ModifiedResult GetApproval(DiplomaticAgreement agreement)
        {
            return ModifiedResult.Create(
                new List<GameModifier>() 
                { 
                    GameModifier.Create(
                        "modifier-diplomacy-base",
                        "Base",
                        SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(10)))
                });
        }

        public ModifiedResult GetApproval(Faction faction)
        {
            CulturalTraits culture = Faction.Culture.Traits;
            CulturalTraits target = faction.Culture.Traits;
            List<GameModifier> modifiers = new();
            if (target.AuthoritarianEgalitarian < 0)
            {
                if (culture.AuthoritarianEgalitarian < 0)
                {
                    modifiers.Add(DiplomacyModifiers.AuthoritarianPositive);
                }
                else if (culture.AuthoritarianEgalitarian > 0)
                {
                    modifiers.Add(DiplomacyModifiers.AuthoritarianNegative);
                }
            }
            else if (target.AuthoritarianEgalitarian > 0)
            {
                if (culture.AuthoritarianEgalitarian < 0)
                {
                    modifiers.Add(DiplomacyModifiers.EgalitarianNegative);
                }
                else if (culture.AuthoritarianEgalitarian > 0)
                {
                    modifiers.Add(DiplomacyModifiers.EgalitarianPositive);
                }
            }

            if (target.IndividualistCollectivist < 0)
            {
                if (culture.IndividualistCollectivist < 0)
                {
                    modifiers.Add(DiplomacyModifiers.IndividualistPositive);
                }
                else if (culture.IndividualistCollectivist > 0)
                {
                    modifiers.Add(DiplomacyModifiers.IndividualistNegative);
                }
            }
            else if (target.IndividualistCollectivist > 0)
            {
                if (culture.IndividualistCollectivist < 0)
                {
                    modifiers.Add(DiplomacyModifiers.CollectivistNegative);
                }
                else if (culture.IndividualistCollectivist > 0)
                {
                    modifiers.Add(DiplomacyModifiers.CollectivistPositive);
                }
            }

            if (target.AggressivePassive < 0)
            {
                if (culture.AggressivePassive < 0)
                {
                    modifiers.Add(DiplomacyModifiers.AggressivePositive);
                }
                else if (culture.AggressivePassive > 0)
                {
                    modifiers.Add(DiplomacyModifiers.AggressiveNegative);
                }
            }
            else if (target.AggressivePassive > 0)
            {
                if (culture.AggressivePassive < 0)
                {
                    modifiers.Add(DiplomacyModifiers.PassiveNegative);
                }
                else if (culture.AggressivePassive > 0)
                {
                    modifiers.Add(DiplomacyModifiers.PassivePositive);
                }
            }

            if (target.ConventionalDynamic < 0)
            {
                if (culture.ConventionalDynamic < 0)
                {
                    modifiers.Add(DiplomacyModifiers.ConventionalPositive);
                }
                else if (culture.ConventionalDynamic > 0)
                {
                    modifiers.Add(DiplomacyModifiers.ConventionalNegative);
                }
            }
            else if (target.ConventionalDynamic > 0)
            {
                if (culture.ConventionalDynamic < 0)
                {
                    modifiers.Add(DiplomacyModifiers.DynamicNegative);
                }
                else if (culture.ConventionalDynamic > 0)
                {
                    modifiers.Add(DiplomacyModifiers.DynamicPositive);
                }
            }

            if (target.MonumentalHumble < 0)
            {
                if (culture.MonumentalHumble < 0)
                {
                    modifiers.Add(DiplomacyModifiers.MonumentalPositive);
                }
                else if (culture.MonumentalHumble > 0)
                {
                    modifiers.Add(DiplomacyModifiers.MonumentalNegative);
                }
            }
            else if (target.MonumentalHumble > 0)
            {
                if (culture.MonumentalHumble < 0)
                {
                    modifiers.Add(DiplomacyModifiers.HumbleNegative);
                }
                else if (culture.MonumentalHumble > 0)
                {
                    modifiers.Add(DiplomacyModifiers.HumblePositive);
                }
            }

            if (target.IndulgentAustere < 0)
            {
                if (culture.IndulgentAustere < 0)
                {
                    modifiers.Add(DiplomacyModifiers.IndulgentPositive);
                }
                else if (culture.IndulgentAustere > 0)
                {
                    modifiers.Add(DiplomacyModifiers.IndulgentNegative);
                }
            }
            else if (target.IndulgentAustere > 0)
            {
                if (culture.IndulgentAustere < 0)
                {
                    modifiers.Add(DiplomacyModifiers.AustereNegative);
                }
                else if (culture.IndulgentAustere > 0)
                {
                    modifiers.Add(DiplomacyModifiers.AusterePositive);
                }
            }

            return ModifiedResult.Create(modifiers);
        }
    }
}
