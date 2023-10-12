using Cardamom.Collections;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class PeaceProposal : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.Peace;
        public ISet<DiplomacyType> TypesToBlock => new EnumSet<DiplomacyType>(DiplomacyType.Peace);
        public ISet<DiplomacyType> TypesToCancel => new EnumSet<DiplomacyType>(DiplomacyType.War);
        public bool IsMirrored => true;
        public bool IsUnilateral => false;

        public void Apply(World world, DiplomaticRelation relation)
        {
            relation.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.Peace);
        }

        public void Cancel(World world, DiplomaticRelation relation) { }

        public void Notify(
            World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer) { }
    }
}
