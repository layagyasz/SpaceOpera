using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationRow : UiSerialContainer
    {
        private static readonly string s_FormationLayerRowClassName = "formation-layer-row";
        private static readonly string s_FormationLayerRowIconClassName = "formation-layer-row-icon";
        private static readonly string s_FormationLayerRowTextClassName = "formation-layer-row-text";

        private readonly List<IFormationDriver> _drivers = new();

        private FormationRow(Class @class, Icon icon, IUiElement text)
            : base(@class, new ButtonController(), Orientation.Horizontal)
        {
            Add(icon);
            Add(text);
        }

        public void Add(IFormationDriver driver)
        {
            _drivers.Add(driver);
        }

        public void Remove(IFormationDriver driver)
        {
            _drivers.Remove(driver);
        }

        public IEnumerable<IFormationDriver> GetDrivers()
        {
            return _drivers;
        }

        public static FormationRow Create(
            object key, string name, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return new(
                uiElementFactory.GetClass(s_FormationLayerRowClassName),
                iconFactory.Create(
                    uiElementFactory.GetClass(s_FormationLayerRowIconClassName),
                    new InlayController(),
                    key),
                new TextUiElement(
                    uiElementFactory.GetClass(s_FormationLayerRowTextClassName), new InlayController(), name));
        }
    }
}
