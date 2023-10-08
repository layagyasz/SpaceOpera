namespace SpaceOpera.Core.Politics.Diplomacy.Statuses
{
    public interface IDiplomaticStatus
    {
        DiplomacyType Type { get; }
        ISet<DiplomacyType> TypesToPrevent { get; }
        void Cancel(World world, DiplomaticRelation relation);
        void Notify(World world, DiplomaticRelation relation, IDiplomaticStatus newStatus);
    }
}
