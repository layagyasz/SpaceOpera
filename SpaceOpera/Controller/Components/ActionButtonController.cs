using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceOpera.View;

namespace SpaceOpera.Controller.Components
{
    public class ActionButtonController : ButtonController, IActionController, IOptionController<ActionId>
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<EventArgs>? Selected { get; set; }

        public object? Object { get; }
        public ActionId Key { get; }

        public ActionButtonController(object? @object, ActionId id)
        {
            Object = @object;
            Key = id;
        }

        public ActionButtonController(ActionId id)
            : this(null, id)
        {
            Key = id;
        }

        public void SetSelected(bool selected)
        {
            SetToggle(selected);
        }

        public override bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                Interacted?.Invoke(
                    this, 
                    UiInteractionEventArgs.Create(
                        Object == null ? Enumerable.Empty<object>() : new List<object>() { Object }, Key));
                Selected?.Invoke(this, EventArgs.Empty);
            }
            return base.HandleMouseButtonClicked(e);
        }
    }
}
