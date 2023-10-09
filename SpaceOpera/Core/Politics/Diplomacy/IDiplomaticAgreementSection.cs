namespace SpaceOpera.Core.Politics.Diplomacy
{
    public interface IDiplomaticAgreementSection
    {
        DiplomacyType Type { get; }
        ISet<DiplomacyType> TypesToBlock { get; }
        ISet<DiplomacyType> TypesToCancel { get; }
        bool IsMirrored { get; }
        bool IsUnilateral { get; }
        void Apply(World world, DiplomaticRelation relation);
        void Cancel(World world, DiplomaticRelation relation);
        void Notify(World world, DiplomaticRelation relation, IDiplomaticAgreementSection agreement, bool isProposer);
    }
}
