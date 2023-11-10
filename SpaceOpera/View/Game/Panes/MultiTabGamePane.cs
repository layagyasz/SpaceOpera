using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Game.Panes
{
    public abstract class MultiTabGamePane : DynamicUiContainer, IBasicGamePane
    {
        public EventHandler<EventArgs>? Populated { get; set; }

        public TextUiElement Header { get; }
        public IUiElement CloseButton { get; }
        public UiCompoundComponent Tabs { get; }
        public IUiElement? Body { get; private set; }

        public MultiTabGamePane(
            IElementController controller, 
            Class @class,
            TextUiElement header, 
            IUiElement closeButton, 
            UiCompoundComponent tabs)
            : base(@class, controller)
        {
            Header = header;
            CloseButton = closeButton;
            Tabs = tabs;

            Add(header);
            closeButton.Position = new(header.Size.X, 0, 0);
            Add(closeButton);
            tabs.Position = new(0, header.Size.Y, 0);
            Add(tabs);
        }

        public abstract void Populate(params object?[] args);

        public void SetBody(IUiElement body)
        {
            if (Body == body)
            {
                return;
            }
            if (Body != null)
            {
                Remove(Body, /* dispose= */ false);
            }
            body.Position = new(0, Tabs.Size.Y + Tabs.Position.Y, 0);
            Add(body);
            Body = body;
        }

        public abstract object GetTab();
        public abstract void SetTab(object id);

        public void SetTitle(string title)
        {
            Header.SetText(title);
        }
    }
}
