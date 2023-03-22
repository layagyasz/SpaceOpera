using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class DesignerComponentCell : UiContainer
    {
        private readonly IconFactory _iconFactory;
        private readonly Class _iconClass;

        public DesignerComponentCell(
            Class @class, IElementController controller, IconFactory iconFactory, Class iconClass)
            : base(@class, controller)
        {
            _iconFactory = iconFactory;
            _iconClass = iconClass;
        }

        public void SetComponent(IComponent? component)
        {
            Clear(true);
            if (component != null)
            {
                var icon = _iconFactory.Create(_iconClass, new InlayController(), component);
                icon.Initialize();
                Add(icon);
            }
        }
    }
}
