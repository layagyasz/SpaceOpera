using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Overlay
{
    public class StarSystemOverlay : DynamicUiCompoundComponent, IOverlay
    {
        private static readonly string s_Container = "overlay-star-system-container";
        private static readonly ActionRow<StellarBody>.Style s_StellarBodyRowStyle =
            new()
            {
                Container = "overlay-star-system-row"
            };
        private static readonly string s_Icon = "overlay-star-system-row-icon";
        private static readonly string s_Text = "overlay-star-system-row-text";

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private float _verticalSpace;

        public StarSystemOverlay(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ActionTableController(), 
                  new UiSerialContainer(
                    uiElementFactory.GetClass(s_Container),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
        }

        public override void Draw(IRenderTarget target, IUiContext context)
        {
            Position = new(0, 0.5f * (_verticalSpace - Size.Y), 0);
            base.Draw(target, context);
        }

        public void Populate(params object?[] args)
        {
            Clear(/* dispose= */ true);
            var starSystem = (StarSystem)args[0]!;
            foreach (var stellarBody in starSystem.Orbiters)
            {
                var row = 
                    ActionRow<StellarBody>.Create(
                        stellarBody,
                        _uiElementFactory,
                        s_StellarBodyRowStyle,
                        new List<IUiElement>()
                        {
                            _iconFactory.Create(
                                _uiElementFactory.GetClass(s_Icon), new InlayController(), stellarBody),
                            new TextUiElement(
                                _uiElementFactory.GetClass(s_Text), new InlayController(), stellarBody.Name)
                        },
                        Enumerable.Empty<ActionRow<StellarBody>.ActionConfiguration>());
                row.Initialize();
                Add(row);
            }
        }

        public override void ResizeContext(Vector3 bounds)
        {
            _verticalSpace = bounds.Y;
        }
    }
}
