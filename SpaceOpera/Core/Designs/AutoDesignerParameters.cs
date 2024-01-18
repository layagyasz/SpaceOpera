namespace SpaceOpera.Core.Designs
{
    public class AutoDesignerParameters
    {
        public ComponentType Type { get; set; }
        public DesignFitness Fitness { get; set; } = new();
    }
}