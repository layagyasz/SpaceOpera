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

        public int Count => _formations.Count;

        private readonly List<IFormation> _formations = new();

        private FormationRow(Class @class, Icon icon, IUiElement text)
            : base(@class, new ButtonController(), Orientation.Horizontal)
        {
            Add(icon);
            Add(text);
        }

        public void Add(IFormation formation)
        {
            _formations.Add(formation);
        }

        public void Remove(IFormation formation)
        {
            _formations.Remove(formation);
        }

        public IEnumerable<IFormation> GetFormations()
        {
            return _formations;
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
