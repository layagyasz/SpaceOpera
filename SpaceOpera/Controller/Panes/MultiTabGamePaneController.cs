using Cardamom.Ui.Controller;
using Cardamom.Ui;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.View.Panes;

namespace SpaceOpera.Controller.Panes
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

        private void HandleClose(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleTabChange(object? sender, ValueChangedEventArgs<string, object?> e)
        {
            ((MultiTabGamePane)_pane!).SetTab(e.Value!);
            _pane!.Refresh();
        }
    }
}
