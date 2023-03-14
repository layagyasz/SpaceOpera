using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;
using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.Controller.Panes;
using SpaceOpera.Controller.Panes.DesignPanes;
using Cardamom.Collections;
using System.Security.Cryptography.X509Certificates;

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

        public UiCompoundComponent ComponentOptionTable { get; }
        public UiCompoundComponent SegmentTable { get; }

        private readonly UiElementFactory _uiElementFactory;
        private IconFactory _iconFactory;

        private World? _world;
        private Faction? _faction;
        private DesignTemplate? _template;

        public DesignerPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                new DesignerPaneController(),
                uiElementFactory.GetClass(s_ClassName),
                new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), string.Empty),
                uiElementFactory.CreateSimpleButton(s_CloseClass).Item1,
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_BodyClassName), 
                    new NoOpElementController<UiSerialContainer>(), 
                    UiSerialContainer.Orientation.Horizontal))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

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
                    new DesignerSegmentTableController(),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_SegmentTableClassName),
                        new TableController(),
                        UiSerialContainer.Orientation.Vertical));
            AddToBody(SegmentTable);
        }

        public override void Populate(params object?[] args)
        {
            var design = args[2] as Design;

            _world = args[0] as World;
            _faction = args[1] as Faction;
            _iconFactory = _iconFactory.ForFaction(_faction!);
            _template = design!.Configuration.Template;
            SetTitle(EnumMapper.ToString(_template.Type));

            ComponentOptionTable.Clear();
            SegmentTable.Clear();
            foreach (var segment in design.Configuration.GetSegments())
            {
                var segmentRow = new DesignerSegmentRow(segment.Template, _uiElementFactory, _iconFactory);
                segmentRow.Initialize();
                segmentRow.Populate(segment.Configuration, segment.GetComponents());
                ((SelectController<SegmentConfiguration>)segmentRow.ConfigurationSelect.Controller)
                    .SetValue(segment.Configuration);
                SegmentTable.Add(segmentRow);
            }
        }

        public void SetSegmentConfiguration(DesignerSegmentRow segmentRow, SegmentConfiguration configuration)
        {
            var defaultComponents = new MultiMap<DesignSlot, IComponent>();
            foreach (var slot in configuration.Slots)
            {
                defaultComponents.Add(
                    slot,
                    Enumerable.Repeat(
                        _world!.GetComponentsFor(_faction!).Where(x => x.FitsSlot(slot)).First(), slot.Count));
            }
            segmentRow.Populate(configuration, defaultComponents);
        }

        public void SetSlot(DesignSlot? slot)
        {
            ComponentOptionTable.Clear();
            if (slot != null)
            {
                foreach (var component in _world!.GetComponentsFor(_faction!).Where(x => x.FitsSlot(slot.Value)))
                {
                    var option = DesignerComponentOption.Create(component, _uiElementFactory, _iconFactory);
                    option.Initialize();
                    ComponentOptionTable.Add(option);
                }
            }
        }
    }
}
