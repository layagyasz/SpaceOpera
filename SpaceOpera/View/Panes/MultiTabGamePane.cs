using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Panes
{
    public abstract class MultiTabGamePane : UiContainer, IGamePane
    {
        public EventHandler<EventArgs>? Populated { get; set; }

        public TextUiElement Header { get; }
        public IUiElement CloseButton { get; }
        public UiCompoundComponent Tabs { get; }
        public IUiContainer Body { get; }

        public MultiTabGamePane(
            IElementController controller, 
            Class @class,
            TextUiElement header, 
            IUiElement closeButton, 
            UiCompoundComponent tabs,
            IUiContainer body) 
            : base(@class, controller)
        {
            Header = header;
            CloseButton = closeButton;
            Tabs = tabs;
            Body = body;

            Add(header);
            closeButton.Position = new(header.Size.X, 0, 0);
            Add(closeButton);
            tabs.Position = new(0, header.Size.Y, 0);
            Add(tabs);
            body.Position = new(0, tabs.Size.Y + tabs.Position.Y, 0);
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

        public abstract void SetTab(object id);

        public void SetTitle(string title)
        {
            Header.SetText(title);
        }
    }
}
