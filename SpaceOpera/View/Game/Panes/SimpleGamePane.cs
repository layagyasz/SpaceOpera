using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Game.Panes
{
    public abstract class SimpleGamePane : DynamicUiContainer, IBasicGamePane
    {
        public EventHandler<EventArgs>? Populated { get; set; }

        public TextUiElement Header { get; }
        public IUiElement CloseButton { get; }
        public IUiElement? Body { get; private set; }

        public SimpleGamePane(
            IElementController controller,
            Class @class,
            TextUiElement header,
            IUiElement closeButton)
            : base(@class, controller)
        {
            Header = header;
            CloseButton = closeButton;

            Add(header);
            closeButton.Position = new(header.Size.X, 0, 0);
            Add(closeButton);
        }

        public abstract void Populate(params object?[] args);

        public void SetBody(IUiElement body)
        {
            if (Body != null)
            {
                Remove(Body, /* dispose= */ false);
            }
            body.Position = new(0, Header.Size.Y, 0);
            Add(body);
            Body = body;
        }

        public void SetTitle(string title)
        {
            Header.SetText(title);
        }
    }
}
