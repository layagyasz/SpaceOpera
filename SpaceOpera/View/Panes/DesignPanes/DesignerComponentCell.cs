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

        public DesignerComponentCell(Class @class, IconFactory iconFactory, Class iconClass)
            : base(@class, new ButtonController())
        {
            _iconFactory = iconFactory;
            _iconClass = iconClass;
        }

        public void SetComponent(IComponent? component)
        {
            Clear();
            if (component != null)
            {
                var icon = _iconFactory.Create(_iconClass, new ButtonController(), component);
                icon.Initialize();
                Add(icon);
            }
        }
    }
}
