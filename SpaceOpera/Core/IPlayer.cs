using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core
{
    public interface IPlayer
    {
        Faction Faction { get; }

        ModifiedResult? GetApproval(Faction other);
    }
}
