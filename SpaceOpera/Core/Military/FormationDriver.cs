using Cardamom.Graphing.BehaviorTree;
using SpaceOpera.Core.Military.Ai;
using SpaceOpera.Core.Military.Ai.Actions;
using SpaceOpera.Core.Universe;
using static SpaceOpera.Core.Military.Ai.SpaceOperaContext;

namespace SpaceOpera.Core.Military
{
    public abstract class FormationDriver
    {
        public EventHandler<MovementEventArgs>? Moved { get; set; }
        public EventHandler<EventArgs>? OrderUpdated { get; set; }

        public IFormation Formation { get; }

        private readonly IFormationAi _ai;
        private IAction? _action;

        protected FormationDriver(IFormation formation, IFormationAi ai)
        {
            Formation = formation;
            Formation.Moved += HandleMove;

            _ai = ai;
        }

        public ICollection<INavigable> GetActiveRegion()
        {
            return _ai.GetActiveRegion();
        }

        public void SetActiveRegion(IEnumerable<INavigable> activeRegion)
        {
            _ai.SetActiveRegion(activeRegion);
            OrderUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void Tick(SpaceOperaContext context)
        {
            Formation.Cohere();
            var newAction = _ai.Execute(context.ForFormation(Formation)).Result;
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