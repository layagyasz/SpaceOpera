using Cardamom.Collections;

namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class DefensePact : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.DefensePact;
        public ISet<DiplomacyType> TypesToBlock => new EnumSet<DiplomacyType>(DiplomacyType.DefensePact);
        public ISet<DiplomacyType> TypesToCancel => new EnumSet<DiplomacyType>();
        public bool IsMirrored => true;
        public bool IsUnilateral => false;

        public void Apply(World world, DiplomaticRelation relation) { }

        public void Cancel(World world, DiplomaticRelation relation) { }

        public void Notify(
            World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer)
        {
            if (isProposer && agreement is WarDeclaration)
            {
                // Implement event
            }
        }
    }
}
