﻿using Cardamom.Ui;
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
        private static readonly string s_Container = "design-pane";
        private static readonly string s_Title = "design-pane-title";
        private static readonly string s_Close = "design-pane-close";
        private static readonly string s_TabContainer = "design-pane-tab-container";
        private static readonly string s_TabOption = "design-pane-tab-option";
        private static readonly string s_Body = "design-pane-body";
        private static readonly string s_DesignTable = "design-pane-design-table";

        private static readonly ActionRow<Design>.Style s_DesignRowStyle =
            new()
            {
                Container = "design-pane-component-row",
                ActionContainer = "design-pane-component-row-action-container"
            };
        private static readonly string s_Icon = "design-pane-component-row-icon";
        private static readonly string s_Text = "design-pane-component-row-text";
        private static readonly List<ActionRow<Design>.ActionConfiguration> s_DesignActions =
            new()
            {
                new ()
                {
                    Button = "design-pane-component-row-action-edit",
                    Action = ActionId.Edit
                }
            };

        private World? _world;
        private Faction? _faction;
        private ComponentType _componentType;

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public UiCompoundComponent DesignTable { get; }

        protected DesignPane(
            UiElementFactory uiElementFactory, IconFactory iconFactory, IEnumerable<ComponentType> componentTypes)
            : base(
                new DesignPaneController(),
                uiElementFactory.GetClass(s_Container),
                new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Designs"),
                uiElementFactory.CreateSimpleButton(s_Close).Item1,
                TabBar<ComponentType>.Create(
                    componentTypes.Select(x => new TabBar<ComponentType>.Definition(x, EnumMapper.ToString(x))),
                    uiElementFactory.GetClass(s_TabContainer),
                    uiElementFactory.GetClass(s_TabOption)))
        {
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            var body = new
                DynamicUiContainer(
                    uiElementFactory.GetClass(s_Body), new NoOpElementController<UiContainer>());
            DesignTable =
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    new DynamicKeyedTable<Design, ActionRow<Design>>(
                        uiElementFactory.GetClass(s_DesignTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        GetRange,
                        CreateRow,
                        Comparer<Design>.Create((x, y) => x.Name.CompareTo(y.Name))));
            body.Add(DesignTable);
            SetBody(body);
        }

        public override void Populate(params object?[] args)
        {
            _world = args[0] as World;
            _faction = args[1] as Faction;
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }

        public override void SetTab(object id)
        {
            _componentType = (ComponentType)id;
        }

        private ActionRow<Design> CreateRow(Design design)
        {
            return ActionRow<Design>.Create(
                design,
                ActionId.Unknown,
                _uiElementFactory,
                s_DesignRowStyle, 
                new List<IUiElement>() 
                {
                    _iconFactory.Create(_uiElementFactory.GetClass(s_Icon), new InlayController(), design), 
                    new TextUiElement(
                        _uiElementFactory.GetClass(s_Text), new InlayController(), design.Name)
                }, 
                s_DesignActions);
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
