namespace SpaceOpera.Core.Politics
{
    public class DiplomaticRelation
    {
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
    }
}