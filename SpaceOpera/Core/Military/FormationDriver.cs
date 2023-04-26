using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.SpaceOperaContext;

namespace SpaceOpera.Core.Military
{
    public abstract class FormationDriver
    {
        public EventHandler<MovementEventArgs>? Moved { get; set; }
        public EventHandler<EventArgs>? OrderUpdated { get; set; }

        public IFormation Formation { get; }
        private HashSet<INavigable> _activeRegion = new();

        private readonly ISupplierNode<IAction, FormationContext> _ai;
        private IAction? _action;

        protected FormationDriver(IFormation formation, IFormationAi ai)
        {
            Formation = formation;
            Formation.Moved += HandleMove;

            _ai = ai;
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            return _activeRegion;
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
            _action?.Progress(Formation, context.World);
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            Moved?.Invoke(this, e);
        }
    }
}