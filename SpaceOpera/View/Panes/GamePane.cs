using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Panes
{
    public class GamePane : UiContainer
    {
        public IUiElement Header { get; }
        public IUiElement CloseButton { get; }

        public GamePane(IElementController controller, Class @class, IUiElement header, IUiElement closeButton) 
            : base(@class, controller)
        {
            Header = header;
            CloseButton = closeButton;
            Add(header);
            closeButton.Position = new(header.Size.X, 0, 0);
            Add(closeButton);
        }
    }
}
