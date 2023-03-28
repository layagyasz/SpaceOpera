using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationList : UiSerialContainer
    {
        private static readonly string s_FormationLayerTableClassName = "formation-layer-table";

        private readonly Vector3 _position;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly Dictionary<object, FormationRow> _rows = new();

        public FormationList(Vector3 position, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  uiElementFactory.GetClass(s_FormationLayerTableClassName),
                  new TableController(0), 
                  Orientation.Vertical)
        {
            _position = position;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
        }

        public void Add(IFormation formation)
        {
            var row = 
                FormationRow.Create(
                    formation, formation.Name, _uiElementFactory, _iconFactory.ForFaction(formation.Faction));
            row.Add(formation);
            _rows.Add(formation, row);
            Add(row);
        }

        public override void Draw(RenderTarget target, UiContext context)
        {
            base.Draw(target, context);
        }

        public void Remove(IFormation formation)
        {
            if (_rows.TryGetValue(formation, out var row))
            {
                Remove(row);
                _rows.Remove(formation);
                row.Dispose();
            }
        }

        public void UpdateFromCamera(Matrix4 camera, UiContext context)
        {
            var transformed = new Vector4(_position, 1) * camera;
            transformed /= transformed.W;
            var window = context.NdcToWindow(transformed.Xy);
            Position = new Vector3(window.X, window.Y, 0) - 0.5f * Size;
        }
    }
}
