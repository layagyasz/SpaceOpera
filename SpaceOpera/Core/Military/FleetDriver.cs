using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Actions;
using SpaceOpera.Core.Military.Routines;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military
{
    public class FleetDriver : IFormationDriver
    {
        public EventHandler<MovementEventArgs>? Moved { get; set; }
        public EventHandler<EventArgs>? OrderUpdated { get; set; }

        public IFormation Formation { get; }
        public FleetAssignment Assignment { get; private set; }
        private HashSet<INavigable> _activeRegion = new();

        private readonly ISupplierNode<IAction, FleetContext> _ai;
        private IAction? _action;

        public FleetDriver(IFormation formation)
        {
            Formation = formation;
            Formation.Moved += HandleMove;

            _ai = FleetRoutine.Create();
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            return _activeRegion;
        }

        public void SetAssignment(FleetAssignment action)
        {
            Assignment = action;
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void SetActiveRegion(IEnumerable<INavigable> activeRegion)
        {
            _activeRegion = new HashSet<INavigable>(activeRegion);
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Tick(SpaceOperaContext context)
        {
            Formation.Cohere();
            var newAction = _ai.Execute(context.ForFleet(this)).Result;
            if (newAction == null || _action == null || !newAction.Equivalent(_action))
            {
                _action = newAction;
            }
            if (_action != null)
            {
                Console.WriteLine("{0} : {1}", Formation.Name, _action);
            }
            _action?.Progress(Formation, context.World);
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            Moved?.Invoke(this, e);
        }
    }
}