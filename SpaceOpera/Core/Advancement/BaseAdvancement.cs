namespace SpaceOpera.Core.Advancement
{
    public class BaseAdvancement : IAdvancement
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public AdvancementType? Type { get; set; }
        public float Cost { get; set; }
        public List<IAdvancement> Prerequisites { get; set; } = new();
    }
}
