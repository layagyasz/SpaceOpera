using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
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

        class StellarBodyRange : IRange<StellarBody>
        {
            public StarSystem? StarSystem { get; set; }

            public string GetHeading()
            {
                return StarSystem?.Name ?? string.Empty;
            }

            public IEnumerable<StellarBody> GetRange()
            {
                return StarSystem?.Orbiters ?? Enumerable.Empty<StellarBody>();
            }
        }

        private StellarBodyRange _range = new();
        private float _verticalSpace;

        public StarSystemOverlay(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ActionComponentController(),
                  new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical))
        {
            var stellarBodyTable =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_TableContainer),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        new DynamicTextUiElement(
                            uiElementFactory.GetClass(s_TableHeader), new ButtonController(), _range.GetHeading),
                        new DynamicUiCompoundComponent(
                            new ActionComponentController(),
                            new DynamicKeyedTable<StellarBody>(
                                uiElementFactory.GetClass(s_Table),
                                new TableController(10f),
                                UiSerialContainer.Orientation.Vertical,
                                _range,
                                new SimpleKeyedElementFactory<StellarBody>(uiElementFactory, iconFactory, CreateRow),
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
            _range.StarSystem = (StarSystem)args[0]!;
            Refresh();
        }

        public override void ResizeContext(Vector3 bounds)
        {
            _verticalSpace = bounds.Y;
        }

        private static ActionRow<StellarBody> CreateRow(
            StellarBody stellarBody, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return
                ActionRow<StellarBody>.Create(
                    stellarBody,
                    ActionId.Select,
                    ActionId.Unknown,
                    uiElementFactory,
                    s_RowStyle,
                    new List<IUiElement>()
                    {
                        iconFactory.Create(
                            uiElementFactory.GetClass(s_Icon), new InlayController(), stellarBody),
                        new TextUiElement(
                            uiElementFactory.GetClass(s_Text), new InlayController(), stellarBody.Name)
                    },
                    Enumerable.Empty<ActionRow<StellarBody>.ActionConfiguration>());
        }
    }
}
