using Cardamom.Trackers;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military
{
    public interface IAtomicFormation : IFormation
    {
        EventHandler<MovementEventArgs>? Moved { get; set; }

        INavigable? Position { get; }
        Pool Cohesion { get; }
        List<UnitGrouping> Composition { get; }
        Inventory Inventory { get; }
        bool InCombat { get; }

        void CheckInventory();
        void Cohere();
        void EnterCombat();
        void ExitCombat();
        float GetCommand();
        float GetSpeed(NavigableEdgeType Type);
        void SetPosition(INavigable Position);
    }
}