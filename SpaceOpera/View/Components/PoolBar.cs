using Cardamom.Trackers;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components
{
    public class PoolBar : SimpleUiElement, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        private readonly Pool _pool;

        public PoolBar(Class @class, IElementController controller, Pool pool)
            : base(@class, controller)
        {
            _pool = pool;
        }

        public void Refresh()
        {
            var p = _pool.PercentFull();
            SetDynamicSize(
                new(
                    p * SizeDefinition.Width.MaximumSize + (1 - p) * SizeDefinition.Width.MinimumSize,
                    SizeDefinition.Height.Size, 
                    0));
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
