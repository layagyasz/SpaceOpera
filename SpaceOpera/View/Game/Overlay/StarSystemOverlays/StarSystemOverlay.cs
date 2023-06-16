using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Components;
using SpaceOpera.View.Game;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Overlay.StarSystemOverlays
{
    public class StarSystemOverlay : DynamicUiCompoundComponent, IOverlay
    {
        private static readonly string s_Container = "star-system-overlay-container";
        private static readonly string s_TableContainer = "empire-overlay-table-container";
        private static readonly string s_TableHeader = "empire-overlay-table-header";
        private static readonly string s_Table = "empire-overlay-table";
        private static readonly ActionRow<StellarBody>.Style s_RowStyle =
            new()
            {
                Container = "star-system-overlay-row"
            };
        private static readonly string s_Icon = "star-system-overlay-row-icon";
        private static readonly string s_Text = "star-system-overlay-row-text";

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private StarSystem? _starSystem;
        private float _verticalSpace;

        public StarSystemOverlay(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ActionComponentController(),
                  new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            var stellarBodyTable =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new DynamicTextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), GetHeading),
                        new DynamicUiCompoundComponent(
                            new ActionComponentController(),
                            new DynamicKeyedTable<StellarBody, ActionRow<StellarBody>>(
                                uiElementFactory.GetClass(s_Table),
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                GetRange,
                                CreateRow,
                                Comparer<StellarBody>.Create(
                                    (x, y) => x.Name.CompareTo(y.Name))))
                    });
            Add(stellarBodyTable);
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            Position = new(0, 0.5f * (_verticalSpace - Size.Y), 0);
            base.Draw(target, context);
        }

        public void Populate(params object?[] args)
        {
            _starSystem = (StarSystem)args[0]!;
            Refresh();
        }

        public override void ResizeContext(Vector3 bounds)
        {
            _verticalSpace = bounds.Y;
        }

        private string GetHeading()
        {
            return _starSystem?.Name ?? string.Empty;
        }

        private IEnumerable<StellarBody> GetRange()
        {
            return _starSystem?.Orbiters ?? Enumerable.Empty<StellarBody>();
        }

        private ActionRow<StellarBody> CreateRow(StellarBody stellarBody)
        {
            return
                ActionRow<StellarBody>.Create(
                    stellarBody,
                    ActionId.Select,
                    _uiElementFactory,
                    s_RowStyle,
                    new List<IUiElement>()
                    {
                        _iconFactory.Create(
                            _uiElementFactory.GetClass(s_Icon), new InlayController(), stellarBody),
                        new TextUiElement(
                            _uiElementFactory.GetClass(s_Text), new InlayController(), stellarBody.Name)
                    },
                    Enumerable.Empty<ActionRow<StellarBody>.ActionConfiguration>());
        }
    }
}
