namespace SpaceOpera.Core.Politics.Diplomacy.Proposals
{
    public interface IDiplomaticProposalSection
    {
        DiplomacyType Type { get; }
        ISet<DiplomacyType> TypesToCancel { get; }
        bool IsUnilateral { get; }
        void Apply(World world, DiplomaticRelation left, DiplomaticRelation right);
    }
}
