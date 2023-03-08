using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Panes;

namespace SpaceOpera.Controller.Panes
{
    public class GamePaneController : PaneController, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        protected GamePane? _pane;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane = @object as GamePane;
            _pane!.CloseButton.Controller.Clicked += HandleClose;
            ((RadioController<object>)_pane!.Tabs.ComponentController).ValueChanged += HandleTabChange;
        }

        public override void Unbind()
        {
            _pane!.CloseButton.Controller.Clicked -= HandleClose;
            ((RadioController<object>)_pane!.Tabs.Controller).ValueChanged -= HandleTabChange;
            base.Unbind();
        }

        private void HandleClose(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleTabChange(object? sender, ValueChangedEventArgs<string, object> e)
        {
            _pane!.SetTab(e.Value);
            _pane!.Refresh();
        }
    }
}
