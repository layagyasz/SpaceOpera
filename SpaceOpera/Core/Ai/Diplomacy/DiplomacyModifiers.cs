namespace SpaceOpera.Core.Ai.Diplomacy
{
    public static class DiplomacyModifiers
    {
        public static readonly GameModifier AuthoritarianNegative =
            GameModifier.Create(
                "modifier-diplomacy-authoritarian-negative",
                "Authoritarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier AuthoritarianPositive =
            GameModifier.Create(
                "modifier-diplomacy-authoritarian-positive",
                "Authoritarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        public static readonly GameModifier EgalitarianNegative =
            GameModifier.Create(
                "modifier-diplomacy-egalitarian-negative",
                "Egalitarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier EgalitarianPositive =
            GameModifier.Create(
                "modifier-diplomacy-egalitarian-positive",
                "Egalitarian",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        public static readonly GameModifier IndividualistNegative =
            GameModifier.Create(
                "modifier-diplomacy-individualist-negative",
                "Individualist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier IndividualistPositive =
            GameModifier.Create(
                "modifier-diplomacy-individualist-positive",
                "Individualist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        public static readonly GameModifier CollectivistNegative =
            GameModifier.Create(
                "modifier-diplomacy-collectivist-negative",
                "Collectivist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier CollectivistPositive =
            GameModifier.Create(
                "modifier-diplomacy-collectivist-positive",
                "Collectivist",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        public static readonly GameModifier AggressiveNegative =
            GameModifier.Create(
                "modifier-diplomacy-aggressive-negative",
                "Aggressive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier AggressivePositive =
            GameModifier.Create(
                "modifier-diplomacy-aggressive-positive",
                "Aggressive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        public static readonly GameModifier PassiveNegative =
            GameModifier.Create(
                "modifier-diplomacy-passive-negative",
                "Passive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier PassivePositive =
            GameModifier.Create(
                "modifier-diplomacy-passive-positive",
                "Passive",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        public static readonly GameModifier ConventionalNegative =
            GameModifier.Create(
                "modifier-diplomacy-conventional-negative",
                "Conventional",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier ConventionalPositive =
            GameModifier.Create(
                "modifier-diplomacy-conventional-positive",
                "Conventional",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        public static readonly GameModifier DynamicNegative =
            GameModifier.Create(
                "modifier-diplomacy-dynamic-negative",
                "Dynamic",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier DynamicPositive =
            GameModifier.Create(
                "modifier-diplomacy-dynamic-positive",
                "Dynamic",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        public static readonly GameModifier MonumentalNegative =
            GameModifier.Create(
                "modifier-diplomacy-monumental-negative",
                "Monumental",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier MonumentalPositive =
            GameModifier.Create(
                "modifier-diplomacy-monumental-positive",
                "Monumental",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        public static readonly GameModifier HumbleNegative =
            GameModifier.Create(
                "modifier-diplomacy-humble-negative",
                "Humble",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier HumblePositive =
            GameModifier.Create(
                "modifier-diplomacy-humble-positive",
                "Humble",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));

        public static readonly GameModifier IndulgentNegative =
            GameModifier.Create(
                "modifier-diplomacy-indulgent-negative",
                "Indulgent",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier IndulgentPositive =
            GameModifier.Create(
                "modifier-diplomacy-indulgent-positive",
                "Indulgent",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
        public static readonly GameModifier AustereNegative =
            GameModifier.Create(
                "modifier-diplomacy-austere-negative",
                "Austere",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(-5)));
        public static readonly GameModifier AusterePositive =
            GameModifier.Create(
                "modifier-diplomacy-austere-positive",
                "Austere",
                SingleGameModifier.Create(ModifierType.Diplomatic, Modifier.FromConstant(5)));
    }
}
