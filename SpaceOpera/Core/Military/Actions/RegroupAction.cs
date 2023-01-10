using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Actions
{
    class RegroupAction : IAction
    {
        public ActionStatus Status { get; private set; } = ActionStatus.IN_PROGRESS;

        public bool Equivalent(IAction Action)
        {
            if (Action is RegroupAction)
            {
                return true;
            }
            return false;
        }

        public void Progress(IFormation Formation, World World)
        {
            if (Formation.Cohesion.IsFull())
            {
                Status = ActionStatus.DONE;
            }
        }

        public override string ToString()
        {
            return "[RegroupAction]";
        }
    }
}

