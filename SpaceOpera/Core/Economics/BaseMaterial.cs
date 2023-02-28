namespace SpaceOpera.Core.Economics
{
    public class BaseMaterial : IMaterial
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public float Mass { get; set; }
        public float Size { get; set; }
        public MaterialType Type { get; set; } = MaterialType.MaterialContinuous;
    }
}
