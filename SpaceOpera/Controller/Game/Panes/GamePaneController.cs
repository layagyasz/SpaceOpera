using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes;

namespace SpaceOpera.Controller.Game.Panes
{
    public class GamePaneController : PaneController, IGamePaneController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IInterceptor>? InterceptorCreated { get; set; }
        public EventHandler<IInterceptor>? InterceptorCancelled { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        protected IBasicGamePane? _pane;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane = (IBasicGamePane)@object;
            _pane!.CloseButton.Controller.Clicked += HandleClose;
        }

        public override void Unbind()
        {
            _pane!.CloseButton.Controller.Clicked -= HandleClose;
            base.Unbind();
        }

        public virtual void Close()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void HandleClose(object? sender, MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Close();
            }
        }
    }
}
