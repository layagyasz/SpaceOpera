namespace SpaceOpera.Core
{
    public class ModifiedResult
    {
        public float Result { get; }
        public List<GameModifier> Modifiers { get; }

        private ModifiedResult(float result, List<GameModifier> modifiers)
        {
            Result = result;
            Modifiers = modifiers;
        }

        public static ModifiedResult Create(IEnumerable<GameModifier> modifiers)
        {
            return new ModifiedResult(
                modifiers.SelectMany(x => x.Modifiers).Aggregate(Modifier.Zero, (x, y) => x + y.Modifier).GetTotal(),
                modifiers.ToList());
        }
    }
}
