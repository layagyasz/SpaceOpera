using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Panes
{
    public abstract class GamePane : UiContainer
    {
        public IUiElement Header { get; }
        public IUiElement CloseButton { get; }
        public UiComponent Tabs { get; }

        public GamePane(
            IElementController controller, Class @class, IUiElement header, IUiElement closeButton, UiComponent tabs) 
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

        public abstract void SetTab(object id);
    }
}
