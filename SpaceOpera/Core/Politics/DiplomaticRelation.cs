using SpaceOpera.Core.Politics.Diplomacy;

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
        public DiplomaticStatus OverallStatus { get; private set; } = DiplomaticStatus.War;
        public IList<DiplomaticAgreement> CurrentAgreements => _currentAgreements.AsReadOnly();

        private readonly List<DiplomaticAgreement> _currentAgreements = new();

        public DiplomaticRelation(Faction faction, Faction target)
        {
            Faction = faction;
            Target = target;
        }

        public void Add(DiplomaticAgreement agreement)
        {
            _currentAgreements.Add(agreement);
        }

        public void Cancel(DiplomaticAgreement agreement)
        {
            _currentAgreements.Remove(agreement);
        }

        internal void SetOverallStatus(DiplomaticStatus status)
        {
            OverallStatus = status;
        }
    }
}