namespace SpaceOpera.Core.Politics.Diplomacy
{
    public class DefensePact : IDiplomaticAgreementSection
    {
        public DiplomacyType Type => DiplomacyType.DefensePact;

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

        public bool Validate(World world)
        {
            return true;
        }
    }
}
