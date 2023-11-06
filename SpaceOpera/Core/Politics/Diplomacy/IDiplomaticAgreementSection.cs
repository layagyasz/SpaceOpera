namespace SpaceOpera.Core.Politics.Diplomacy
{
    public interface IDiplomaticAgreementSection
    {
        DiplomacyType Type { get; }
        void Apply(World world, DiplomaticRelation relation);
        void Cancel(World world, DiplomaticRelation relation);
        void Notify(World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer);
        bool Validate(World world);
    }
}
