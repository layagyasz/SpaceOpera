namespace SpaceOpera.Core.Economics
{
    public class EconomicSubzone
    {
        public EconomicZone Parent { get; set; }

        public EconomicSubzone(EconomicZone parent)
        {
            Parent = parent;
        }
    }
}
