using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Events
{
    public class FormationDestroyedEvent : NotificationBase
    {
        public override Faction Faction => Formation.Faction;
        public IFormation Formation { get; }

        public FormationDestroyedEvent(IFormation formation)
        {
            Formation = formation;
        }

        public override string GetTitle()
        {
            return "Formation Destroyed";
        }

        public override string GetDescription()
        {
            return $"{Formation.Name} was destroyed in battle.";
        }
    }
}
