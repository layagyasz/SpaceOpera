using Cardamom.Ui.Controller.Element;
using Cardamom.Ui;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Panes
{
    public abstract class SimpleGamePane : UiContainer, IGamePane
    {
        public EventHandler<EventArgs>? Populated { get; set; }

        public TextUiElement Header { get; }
        public IUiElement CloseButton { get; }
        public IUiContainer Body { get; }

        public SimpleGamePane(
            IElementController controller,
            Class @class,
            TextUiElement header,
            IUiElement closeButton,
            IUiContainer body)
            : base(@class, controller)
        {
            Header = header;
            CloseButton = closeButton;
            Body = body;

            Add(header);
            closeButton.Position = new(header.Size.X, 0, 0);
            Add(closeButton);
            body.Position = new(0, header.Size.Y, 0);
            Add(body);
        }

        public void AddToBody(IUiElement element)
        {
            Body.Add(element);
        }

        public void Refresh()
        {
            if (Body is IDynamic body)
            {
                body.Refresh();
            }
        }

        public abstract void Populate(params object?[] args);

        public void SetTitle(string title)
        {
            Header.SetText(title);
        }
    }
}
