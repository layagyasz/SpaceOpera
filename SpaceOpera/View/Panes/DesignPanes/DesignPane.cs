using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Panes.DesignPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public abstract class DesignPane : MultiTabGamePane
    {
        private static readonly string s_Title = "Designs";

        private static readonly string s_ClassName = "design-pane";
        private static readonly string s_TitleClassName = "design-pane-title";
        private static readonly string s_CloseClass = "design-pane-close";
        private static readonly string s_TabContainerClassName = "design-pane-tab-container";
        private static readonly string s_TabOptionClassName = "design-pane-tab-option";
        private static readonly string s_BodyClassName = "design-pane-body";
        private static readonly string s_DesignTableClassName = "design-pane-design-table";

        private World? _world;
        private Faction? _faction;
        private ComponentType _componentType;

        private IconFactory _iconFactory;

        public IUiContainer DesignTable { get; }

        protected DesignPane(
            UiElementFactory uiElementFactory, IconFactory iconFactory, IEnumerable<ComponentType> componentTypes)
            : base(
                new DesignPaneController(),
                uiElementFactory.GetClass(s_ClassName),
                new TextUiElement(uiElementFactory.GetClass(s_TitleClassName), new ButtonController(), s_Title),
                uiElementFactory.CreateSimpleButton(s_CloseClass).Item1, 
                TabBar<ComponentType>.Create(
                    componentTypes.Select(x => new TabBar<ComponentType>.Definition(x, EnumMapper.ToString(x))),
                    uiElementFactory.GetClass(s_TabContainerClassName),
                    uiElementFactory.GetClass(s_TabOptionClassName)),
                new DynamicUiContainer(
                    uiElementFactory.GetClass(s_BodyClassName), new NoOpElementController<UiContainer>())) 
        {
            _iconFactory = iconFactory;
            DesignTable =
                new DynamicUiSerialContainer<Design, DesignRow>(
                    uiElementFactory.GetClass(s_DesignTableClassName), 
                    new ActionTableController(),
                    UiSerialContainer.Orientation.Vertical,
                    GetRange,
                    x => DesignRow.Create(x, uiElementFactory, ref _iconFactory),
                    Comparer<Design>.Create((x, y) => x.Name.CompareTo(y.Name)));
            AddToBody(DesignTable);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            _iconFactory = _iconFactory.ForFaction(_faction!);
            Refresh();
        }

        public override void SetTab(object id)
        {
            _componentType = (ComponentType)id;
        }

        private IEnumerable<Design> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<Design>();
            }
            return _world.GetDesignsFor(_faction).Where(x => x.Configuration.Template.Type == _componentType);
        }
    }
}
