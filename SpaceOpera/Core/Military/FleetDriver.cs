using SpaceOpera.Core.Military.Actions;
using SpaceOpera.Core.Military.Routines;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military
{
    public class FleetDriver
    {
        public EventHandler<EventArgs>? OnOrderUpdated { get; set; }

        public Fleet Fleet { get; }
        public FleetAssignment Assignment { get; private set; }
        private HashSet<INavigable> _ActiveRegion { get; set; } = new();

        private ISupplierNode<IAction, FleetContext> _ai;
        private IAction? _action;

        public FleetDriver(Fleet fleet)
        {
            Fleet = fleet;

            _ai = FleetRoutine.Create();
        }

        public IEnumerable<INavigable> GetActiveRegion()
        {
            return _ActiveRegion;
        }

        public void SetAssignment(FleetAssignment action)
        {
            Assignment = action;
            OnOrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void SetActiveRegion(IEnumerable<INavigable> activeRegion)
        {
            _ActiveRegion = new HashSet<INavigable>(activeRegion);
            OnOrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Tick(SpaceOperaContext context)
        {
            Fleet.Cohere();
            var newAction = _ai.Execute(context.ForFleet(this)).Result;
            if (newAction == null || _action == null || !newAction.Equivalent(_action))
            {
                _action = newAction;
            }
            if (_action != null)
            {
                Console.WriteLine("{0} : {1}", Fleet.Name, _action);
            }
            _action?.Progress(Fleet, context.World);
        }
    }
}