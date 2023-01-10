using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Actions
{
    class SpotAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.IN_PROGRESS;

        public Fleet Target { get; }

        public SpotAction(Fleet Target)
        {
            this.Target = Target;
        }

        public static IAction Create(Fleet Target)
        {
            return new SpotAction(Target);
        }

        public bool Equivalent(IAction Action)
        {
            if (Action is SpotAction other)
            {
                return other.Target == Target;
            }
            return false;
        }

        public void Progress(IFormation Formation, World World)
        {
            World.GetIntelligenceFor(Formation.Faction).FleetIntelligence.Spot(Target, .5);
        }

        public override string ToString()
        {
            return "[SpotAction]";
        }
    }
}