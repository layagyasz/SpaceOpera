using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Panes;

namespace SpaceOpera.Controller.Panes
{
    public class GamePaneController : PaneController, IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        protected IGamePane? _pane;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane = @object as IGamePane;
            _pane!.CloseButton.Controller.Clicked += HandleClose;
        }

        public override void Unbind()
        {
            _pane!.CloseButton.Controller.Clicked -= HandleClose;
            base.Unbind();
        }

        private void HandleClose(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Closed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
