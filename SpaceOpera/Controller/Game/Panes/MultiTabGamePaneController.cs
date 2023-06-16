using Cardamom.Ui.Controller;
using SpaceOpera.View.Game.Panes;

namespace SpaceOpera.Controller.Game.Panes
{
    public class MultiTabGamePaneController : GamePaneController
    {
        public override void Bind(object @object)
        {
            base.Bind(@object);
            var pane = _pane as MultiTabGamePane;
            ((RadioController<object>)pane!.Tabs.ComponentController).ValueChanged += HandleTabChange;
        }

        public override void Unbind()
        {
            var pane = _pane as MultiTabGamePane;
            ((RadioController<object>)pane!.Tabs.Controller).ValueChanged -= HandleTabChange;
            base.Unbind();
        }

        private void HandleTabChange(object? sender, object? e)
        {
            ((MultiTabGamePane)_pane!).SetTab(e!);
            _pane!.Refresh();
        }
    }
}
