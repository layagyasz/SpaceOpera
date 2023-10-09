using Cardamom.Collections;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class WarDeclaration : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.War;
        public ISet<DiplomacyType> TypesToBlock =>
            new EnumSet<DiplomacyType>(DiplomacyType.Exchange, DiplomacyType.Pact);
        public ISet<DiplomacyType> TypesToCancel =>
            new EnumSet<DiplomacyType>(DiplomacyType.Exchange, DiplomacyType.Pact, DiplomacyType.Peace);
        public bool IsMirrored => true;
        public bool IsUnilateral => true;

        public void Apply(World world, DiplomaticRelation relation)
        {
            relation.SetOverallStatus(DiplomaticRelation.DiplomaticStatus.War);
        }

        public void Cancel(World world, DiplomaticRelation relation) { }

        public void Notify(
            World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer) { }
    }
}
