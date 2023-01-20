using SpaceOpera.Core.Economics.Projects;

namespace SpaceOpera.Core.Economics
{
    public class EconomicSubzone : ProjectHub
    {
        public EconomicZone Parent { get; set; }

        public EconomicSubzone(EconomicZone parent)
        {
            Parent = parent;
        }
    }
}
