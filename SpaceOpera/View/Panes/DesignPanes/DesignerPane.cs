using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Controller.Panes.DesignPanes;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.Controller.Panes;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class DesignerPane : SimpleGamePane
    {
        private static readonly string s_ClassName = "designer-pane";
        private static readonly string s_TitleClassName = "designer-pane-title";
        private static readonly string s_CloseClass = "designer-pane-close";
        private static readonly string s_BodyClassName = "designer-pane-body";
        private static readonly string s_ComponentOptionTableClassName = "designer-pane-component-option-table";
        private static readonly string s_SegmentTableClassName = "designer-pane-segment-table";

        public IUiContainer ComponentOptionTable { get; }
        public IUiContainer SegmentTable { get; }

        private World? _world;
        private Faction? _faction;
        private DesignTemplate? _template;

        public DesignerPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                new GamePaneController(),
                uiElementFactory.GetClass(s_ClassName),
                new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), string.Empty),
                uiElementFactory.CreateSimpleButton(s_CloseClass).Item1,
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_BodyClassName), 
                    new NoOpElementController<UiSerialContainer>(), 
                    UiSerialContainer.Orientation.Horizontal))
        {
            ComponentOptionTable =
                new UiCompoundComponent(
                    new RadioController<IComponent>("component"),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_ComponentOptionTableClassName),
                        new TableController(),
                        UiSerialContainer.Orientation.Vertical));
            AddToBody(ComponentOptionTable);
            SegmentTable =
                new UiCompoundComponent(
                    new RadioController<IComponent>("component"),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_SegmentTableClassName),
                        new TableController(),
                        UiSerialContainer.Orientation.Vertical));
            AddToBody(SegmentTable);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            _template = ((Design)args[2]!).Configuration.Template;
            SetTitle(EnumMapper.ToString(_template.Type));
        }
    }
}
