using Cardamom.Collections;

namespace SpaceOpera.Core.Politics.Diplomacy.Statuses
{
    public class AtPeace : IDiplomaticStatus
    {
        public DiplomacyType Type => DiplomacyType.Peace;
        public ISet<DiplomacyType> TypesToPrevent => new EnumSet<DiplomacyType>();
        public void Cancel(World world, DiplomaticRelation relation) { }
        public void Notify(World world, DiplomaticRelation relation, IDiplomaticStatus newStatus) { }
    }
}
