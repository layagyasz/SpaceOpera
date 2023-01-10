using Adansonia;
using SpaceOpera.Core.Military.Actions;
using SpaceOpera.Core.Military.Routines;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military
{
    class FleetDriver
    {
        public EventHandler<EventArgs> OnOrderUpdated { get; set; }

        public Fleet Fleet { get; }
        public IAction Action { get; private set; }
        public FleetAssignment Assignment { get; private set; }
        private HashSet<INavigable> _ActiveRegion { get; set; } = new HashSet<INavigable>();

        private ISupplierNode<IAction, FleetContext> _Ai;
        private IAction _Action;

        public FleetDriver(Fleet Fleet)
        {
            this.Fleet = Fleet;

            _Ai = FleetRoutine.Create();
        }

        public IEnumerable<INavigable> GetActiveRegion()
        {
            return _ActiveRegion;
        }

        public void SetAssignment(FleetAssignment Action)
        {
            this.Assignment = Action;
            OnOrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void SetActiveRegion(IEnumerable<INavigable> ActiveRegion)
        {
            _ActiveRegion = new HashSet<INavigable>(ActiveRegion);
            OnOrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Tick(SpaceOperaContext Context)
        {
            Fleet.Cohere();
            var newAction = _Ai.Execute(Context.ForFleet(this)).Result;
            if (newAction == null || _Action == null || !newAction.Equivalent(_Action))
            {
                _Action = newAction;
            }
            if (_Action != null)
            {
                Console.WriteLine("{0} : {1}", Fleet.Name, _Action);
            }
            _Action?.Progress(Fleet, Context.World);
        }
    }
}