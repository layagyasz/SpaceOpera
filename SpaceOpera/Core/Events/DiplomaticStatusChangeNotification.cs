using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Events
{
    public class DiplomaticStatusChangeNotification : NotificationBase
    {
        public override Faction Faction => Relation.Faction;
        public DiplomaticRelation Relation { get; }
        public DiplomaticRelation.DiplomaticStatus Status { get; }

        public DiplomaticStatusChangeNotification(
            DiplomaticRelation relation, DiplomaticRelation.DiplomaticStatus status)
        {
            Relation = relation;
            Status = status;
        }

        public override string GetTitle()
        {
            return Status == DiplomaticRelation.DiplomaticStatus.War ? "War Declared" : "Peace Treaty Signed";
        }

        public override string GetDescription()
        {
            return "Diplomatic status changed.";
        }
    }
}
