using Cardamom.Collections;

namespace SpaceOpera.Core.Politics.Diplomacy.Statuses
{
    public class AtWar : IDiplomaticStatus
    {
        public DiplomacyType Type => DiplomacyType.War;
        public ISet<DiplomacyType> TypesToPrevent => 
            new EnumSet<DiplomacyType>(DiplomacyType.Exchange, DiplomacyType.Pact);

        public void Cancel(World world, DiplomaticRelation relation)
        {
            if (!relation.Statuses.Any(x => x is AtWar))
            {
                relation.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.Peace);
            }
        }
        public void Notify(World world, DiplomaticRelation relation, IDiplomaticStatus newStatus) { }
    }
}
