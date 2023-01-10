using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    interface IFormation
    {
        EventHandler<MovementEventArgs> OnMoved { get; set; }

        string Name { get; }
        Faction Faction { get; }
        INavigable Position { get; }
        Pool Cohesion { get; }
        List<UnitGrouping> Composition { get; }
        bool InCombat { get; }

        void Cohere();
        void EnterCombat();
        void ExitCombat();
        float GetCommand();
        float GetSpeed(NavigableEdgeType Type);
        void SetPosition(INavigable Position);
    }
}