using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.FormationViews
{
    public class FormationList : DynamicUiCompoundComponent
    {
        private static readonly string s_FormationLayerTable = "formation-layer-table";

        private readonly Vector3 _pin;
        private readonly float? _offset;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly Dictionary<object, FormationRow> _rows = new();

        private Vector4 _position;

        public FormationList(
            Vector3 position, float? offset, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ActionComponentController(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_FormationLayerTable),
                      new TableController(0), 
                      UiSerialContainer.Orientation.Vertical))
        {
            _pin = position;
            _offset = offset;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
        }

        public void Add(AtomicFormationDriver driver, bool initialize)
        {
            (var key, string name) = GetKey(driver);
            if (!_rows.TryGetValue(key, out var row))
            {
                row = new FormationRow(driver, name, _uiElementFactory, _iconFactory);
                if (initialize)
                {
                    row.Initialize();
                }
                _rows.Add(key, row);
                Add(row);
            }
            row.Add(driver);
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            if (_offset == null)
            {
                Position = _position.Xyz - 0.5f * Size;
            }
            else
            {
                Position = new(_position.X - 0.5f * Size.X, _position.Y + _offset.Value / _position.W, _position.Z);
            }
            base.Draw(target, context);
        }

        public IEnumerable<AtomicFormationDriver> GetDrivers()
        {
            return _rows.Values.SelectMany(x => x.GetDrivers());
        }

        public void Remove(AtomicFormationDriver driver)
        {
            (var key, var _) = GetKey(driver);
            if (_rows.TryGetValue(key, out var row))
            {
                row.Remove(driver);
                if (row.FormationCount == 0)
                {
                    Remove(row);
                    _rows.Remove(key);
                    row.Dispose();
                }
            }
        }

        public void UpdateFromCamera(Matrix4 transform)
        {
            var projected = new Vector4(_pin, 1) * transform;
            Visible = projected.Z < 0;
            OverrideDepth = projected.W - 0.05f;
            _position = new(projected.Xyz / projected.W, projected.W);
        }
        
        private static (object, string) GetKey(AtomicFormationDriver driver)
        {
            if (driver is FleetDriver fleet)
            {
                return (fleet, fleet.AtomicFormation.Name);
            }
            if (driver is DivisionDriver division)
            {
                return (((Division)division.AtomicFormation).Template, ((Division)division.AtomicFormation).Template.Name);
            }
            throw new ArgumentException(string.Format("Unsupported driver type {0}.", driver.GetType()));
        }
    }
}
