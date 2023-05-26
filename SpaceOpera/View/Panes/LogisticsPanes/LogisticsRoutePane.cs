using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Panes.LogisticsPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.LogisticsPanes
{
    public class LogisticsRoutePane : SimpleGamePane
    {
        private static readonly string s_Container = "logistics-route-pane";
        private static readonly string s_Title = "logistics-route-pane-title";
        private static readonly string s_Close = "logistics-route-pane-close";
        private static readonly string s_Body = "logistics-route-pane-body";

        private static readonly string s_MaterialsContainer = "logistics-route-pane-materials-container";
        private static readonly string s_MaterialContainer = "logistics-route-pane-material-container";
        private static readonly string s_MaterialHeader = "logistics-route-pane-material-header";

        public MaterialComponent LeftMaterials { get; }
        public MaterialComponent RightMaterials { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private World? _world;
        private Faction? _faction;
        private PersistentRoute? _route;

        public LogisticsRoutePane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new LogisticsRoutePaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Logistics Route"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            LeftMaterials = new MaterialComponent(uiElementFactory, iconFactory);
            RightMaterials = new MaterialComponent(uiElementFactory, iconFactory);
            var materialsContainer =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_MaterialsContainer),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_MaterialContainer), 
                        new NoOpElementController<UiSerialContainer>(), 
                        UiSerialContainer.Orientation.Vertical) 
                    {
                        uiElementFactory.CreateTextButton(s_MaterialHeader, "Left").Item1,
                        LeftMaterials 
                    },
                    new DynamicUiSerialContainer(
                        uiElementFactory.GetClass(s_MaterialContainer),
                        new NoOpElementController<UiSerialContainer>(),
                        UiSerialContainer.Orientation.Vertical)
                    {
                        uiElementFactory.CreateTextButton(s_MaterialHeader, "Right").Item1,
                        RightMaterials
                    },
                };

            var body =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new TableController(10f),
                    UiSerialContainer.Orientation.Vertical)
                {
                    materialsContainer
                };
            SetBody(body);
        }

        public PersistentRoute? GetSeedRoute()
        {
            return _route;
        }

        public override void Populate(params object?[] args)
        {
            _world = (World?)args[0];
            _faction = (Faction?)args[1];
            _route = (PersistentRoute?)args[2];
            LeftMaterials.SetRange(Enumerable.Empty<IMaterial>());
            LeftMaterials.SetOptions(_world?.CoreData.Materials.Values ?? Enumerable.Empty<IMaterial>());
            RightMaterials.SetRange(Enumerable.Empty<IMaterial>());
            RightMaterials.SetOptions(_world?.CoreData.Materials.Values ?? Enumerable.Empty<IMaterial>());
            Populated?.Invoke(this, EventArgs.Empty);
        }
    }
}
