using SpaceOpera.Core.Politics.Cultures;

namespace SpaceOpera.Core.Politics
{
    public class DiplomaticRelation
    {
        private static GameModifier s_AuthoritarianNegative = 
            GameModifier.Create(
                "modifier-diplomacy-authoritarian-negative",
                "Authoritarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_AuthoritarianPositive =
            GameModifier.Create(
                "modifier-diplomacy-authoritarian-positive",
                "Authoritarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        private static GameModifier s_EgalitarianNegative =
            GameModifier.Create(
                "modifier-diplomacy-egalitarian-negative",
                "Egalitarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_EgalitarianPositive =
            GameModifier.Create(
                "modifier-diplomacy-egalitarian-positive",
                "Egalitarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        private static GameModifier s_IndividualistNegative =
            GameModifier.Create(
                "modifier-diplomacy-individualist-negative",
                "Individualist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_IndividualistPositive =
            GameModifier.Create(
                "modifier-diplomacy-individualist-positive",
                "Individualist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        private static GameModifier s_CollectivistNegative =
            GameModifier.Create(
                "modifier-diplomacy-collectivist-negative",
                "Collectivist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_CollectivistPositive =
            GameModifier.Create(
                "modifier-diplomacy-collectivist-positive",
                "Collectivist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        private static GameModifier s_AggressiveNegative =
            GameModifier.Create(
                "modifier-diplomacy-aggressive-negative",
                "Aggressive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_AggressivePositive =
            GameModifier.Create(
                "modifier-diplomacy-aggressive-positive",
                "Aggressive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        private static GameModifier s_PassiveNegative =
            GameModifier.Create(
                "modifier-diplomacy-passive-negative",
                "Passive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_PassivePositive =
            GameModifier.Create(
                "modifier-diplomacy-passive-positive",
                "Passive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        private static GameModifier s_ConventionalNegative =
            GameModifier.Create(
                "modifier-diplomacy-conventional-negative",
                "Conventional",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_ConventionalPositive =
            GameModifier.Create(
                "modifier-diplomacy-conventional-positive",
                "Conventional",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        private static GameModifier s_DynamicNegative =
            GameModifier.Create(
                "modifier-diplomacy-dynamic-negative",
                "Dynamic",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_DynamicPositive =
            GameModifier.Create(
                "modifier-diplomacy-dynamic-positive",
                "Dynamic",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        private static GameModifier s_MonumentalNegative =
            GameModifier.Create(
                "modifier-diplomacy-monumental-negative",
                "Monumental",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_MonumentalPositive =
            GameModifier.Create(
                "modifier-diplomacy-monumental-positive",
                "Monumental",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        private static GameModifier s_HumbleNegative =
            GameModifier.Create(
                "modifier-diplomacy-humble-negative",
                "Humble",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_HumblePositive =
            GameModifier.Create(
                "modifier-diplomacy-humble-positive",
                "Humble",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        private static GameModifier s_IndulgentNegative =
            GameModifier.Create(
                "modifier-diplomacy-indulgent-negative",
                "Indulgent",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_IndulgentPositive =
            GameModifier.Create(
                "modifier-diplomacy-indulgent-positive",
                "Indulgent",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        private static GameModifier s_AustereNegative =
            GameModifier.Create(
                "modifier-diplomacy-austere-negative",
                "Austere",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        private static GameModifier s_AusterePositive =
            GameModifier.Create(
                "modifier-diplomacy-austere-positive",
                "Austere",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        public enum DiplomaticStatus
        {
            None,

            Peace,
            War
        }

        public Faction Faction { get; }
        public Faction Target { get; }
        public DiplomaticStatus Status { get; } = DiplomaticStatus.War;

        public DiplomaticRelation(Faction faction, Faction target)
        {
            Faction = faction;
            Target = target;
        }

        public ModifiedResult GetApproval()
        {
            return CalculateApproval(Target.Culture.Traits, Faction.Culture.Traits);
        }

        private static ModifiedResult CalculateApproval(CulturalTraits culture, CulturalTraits target)
        {
            List<GameModifier> modifiers = new();
            if (target.AuthoritarianEgalitarian < 0)
            {
                if (culture.AuthoritarianEgalitarian < 0)
                {
                    modifiers.Add(s_AuthoritarianPositive);
                }
                else if (culture.AuthoritarianEgalitarian > 0)
                {
                    modifiers.Add(s_AuthoritarianNegative);
                }
            }
            else if (target.AuthoritarianEgalitarian > 0)
            {
                if (culture.AuthoritarianEgalitarian < 0)
                {
                    modifiers.Add(s_EgalitarianNegative);
                }
                else if (culture.AuthoritarianEgalitarian > 0)
                {
                    modifiers.Add(s_EgalitarianPositive);
                }
            }

            if (target.IndividualistCollectivist < 0)
            {
                if (culture.IndividualistCollectivist < 0)
                {
                    modifiers.Add(s_IndividualistPositive);
                }
                else if (culture.IndividualistCollectivist > 0)
                {
                    modifiers.Add(s_IndividualistNegative);
                }
            }
            else if (target.IndividualistCollectivist > 0)
            {
                if (culture.IndividualistCollectivist < 0)
                {
                    modifiers.Add(s_CollectivistNegative);
                }
                else if (culture.IndividualistCollectivist > 0)
                {
                    modifiers.Add(s_CollectivistPositive);
                }
            }

            if (target.AggressivePassive < 0)
            {
                if (culture.AggressivePassive < 0)
                {
                    modifiers.Add(s_AggressivePositive);
                }
                else if (culture.AggressivePassive > 0)
                {
                    modifiers.Add(s_AggressiveNegative);
                }
            }
            else if (target.AggressivePassive > 0)
            {
                if (culture.AggressivePassive < 0)
                {
                    modifiers.Add(s_PassiveNegative);
                }
                else if (culture.AggressivePassive > 0)
                {
                    modifiers.Add(s_PassivePositive);
                }
            }

            if (target.ConventionalDynamic < 0)
            {
                if (culture.ConventionalDynamic < 0)
                {
                    modifiers.Add(s_ConventionalPositive);
                }
                else if (culture.ConventionalDynamic > 0)
                {
                    modifiers.Add(s_ConventionalNegative);
                }
            }
            else if (target.ConventionalDynamic > 0)
            {
                if (culture.ConventionalDynamic < 0)
                {
                    modifiers.Add(s_DynamicNegative);
                }
                else if (culture.ConventionalDynamic > 0)
                {
                    modifiers.Add(s_DynamicPositive);
                }
            }

            if (target.MonumentalHumble < 0)
            {
                if (culture.MonumentalHumble < 0)
                {
                    modifiers.Add(s_MonumentalPositive);
                }
                else if (culture.MonumentalHumble > 0)
                {
                    modifiers.Add(s_MonumentalNegative);
                }
            }
            else if (target.MonumentalHumble > 0)
            {
                if (culture.MonumentalHumble < 0)
                {
                    modifiers.Add(s_HumbleNegative);
                }
                else if (culture.MonumentalHumble > 0)
                {
                    modifiers.Add(s_HumblePositive);
                }
            }

            if (target.IndulgentAustere < 0)
            {
                if (culture.IndulgentAustere < 0)
                {
                    modifiers.Add(s_IndulgentPositive);
                }
                else if (culture.IndulgentAustere > 0)
                {
                    modifiers.Add(s_IndulgentNegative);
                }
            }
            else if (target.IndulgentAustere > 0)
            {
                if (culture.IndulgentAustere < 0)
                {
                    modifiers.Add(s_AustereNegative);
                }
                else if (culture.IndulgentAustere > 0)
                {
                    modifiers.Add(s_AusterePositive);
                }
            }

            return ModifiedResult.Create(modifiers);
        }
    }
}