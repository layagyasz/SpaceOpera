using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Core
{
    public interface IPlayer
    {
        Faction Faction { get; }

        ModifiedResult? GetApproval(Faction other);
        ModifiedResult? GetApproval(DiplomaticAgreement agreement);
        void Tick(World world);
    }
}
