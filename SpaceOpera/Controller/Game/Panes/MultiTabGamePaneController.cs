using Cardamom.Ui.Controller;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes;

namespace SpaceOpera.Controller.Game.Panes
{
    public class MultiTabGamePaneController : GamePaneController
    {
        private IFormFieldController<object>? _tab;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = _pane as MultiTabGamePane;
            _pane!.Populated += HandlePopulated;
            _tab = (RadioController<object>)pane!.Tabs.ComponentController;
            _tab.ValueChanged += HandleTabChange;
            foreach (var tab in pane.GetTabs())
            {
                BindTab((ITabController)tab.ComponentController);
            }
        }

        public override void Unbind()
        {
            foreach (var tab in ((MultiTabGamePane)_pane!).GetTabs())
            {
                UnbindTab((ITabController)tab.ComponentController);
            }
            _tab!.ValueChanged -= HandleTabChange;
            _tab = null;
            _pane.Populated -= HandlePopulated;
            base.Unbind();
        }

        public object GetTab()
        {
            return _tab!.GetValue()!;
        }

        protected virtual void HandlePopulatedImpl() { }

        private void BindTab(ITabController controller)
        {
            controller.Interacted += HandleInteraction;
            controller.OrderCreated += HandleOrder;
        }

        private void UnbindTab(ITabController controller)
        {
            controller.Interacted -= HandleInteraction;
            controller.OrderCreated -= HandleOrder;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }

        private void HandleOrder(object? sender, IOrder e)
        {
            OrderCreated?.Invoke(this, e);
        }

        private void HandlePopulated(object? sender, EventArgs e)
        {
            HandlePopulatedImpl();
        }

        private void HandleTabChange(object? sender, EventArgs e)
        {
            ((MultiTabGamePane)_pane!).SetTab(GetTab());
            _pane!.Refresh();
        }
    }
}
