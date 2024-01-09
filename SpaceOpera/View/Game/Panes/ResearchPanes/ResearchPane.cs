using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Utils;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.ResearchPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.ResearchPanes
{
    public class ResearchPane : SimpleGamePane
    {
        private static readonly string s_Container = "research-pane";
        private static readonly string s_Title = "research-pane-title";
        private static readonly string s_Close = "research-pane-close";
        private static readonly string s_Body = "research-pane-body";

        private static readonly string s_SlotTable = "research-pane-slot-table";
        private static readonly string s_AdvancementTable = "research-pane-advancement-table";
        private static readonly AdvancementComponent.Style s_AdvancementStyle = new()
        {
            Container = "research-pane-advancement-container",
            Icon = "research-pane-advancement-icon",
            Info = "research-pane-advancement-info",
            Text = "research-pane-advancement-text",
            ProgressText = "research-pane-advancement-progress-text",
            Progress = "research-pane-advancement-progress"
        };
        private static readonly AdvancementSlotComponent.Style s_SlotStyle = new()
        {
            Container = "research-pane-slot-table-slot-row",
            EmptyText = "research-pane-slot-table-slot-row-empty",
            Advancement = s_AdvancementStyle
        };

        record class SlotKey(AdvancementSlot Slot, FactionAdvancementManager AdvancementManager);
        record class AdvancementKey(IAdvancement Advancement, FactionAdvancementManager AdvancementManager);

        class AdvancementRange
        {
            private World? _world;
            private Faction? _faction;
            private FactionAdvancementManager? _advancementManager;

            public IEnumerable<AdvancementKey> GetResearchableAdvancements()
            {
                if (_world == null || _faction == null || _advancementManager == null)
                {
                    return Enumerable.Empty<AdvancementKey>();
                }
                else
                {
                    return _world.Advancements.GetResearchableAdvancements(_faction)
                        .Where(x => !_advancementManager!.IsResearching(x))
                        .Select(x => new AdvancementKey(x, _advancementManager));
                }
            }

            public IEnumerable<SlotKey> GetSlots()
            {
                return _advancementManager?.GetAdvancementSlots().Select(x => new SlotKey(x, _advancementManager)) 
                    ?? Enumerable.Empty<SlotKey>();
            }

            public FactionAdvancementManager GetAdvancementManager()
            {
                return _advancementManager!;
            }

            public void Populate(World? world, Faction? faction)
            {
                _world = world;
                _faction = faction;
                if (_world == null || _faction == null)
                {
                    _advancementManager = null;
                }
                else
                {
                    _advancementManager = _world.Advancements.Get(_faction);
                }
            }
        }

        class AdvancementSlotComponentFactory : IKeyedElementFactory<SlotKey>
        {
            private readonly AdvancementSlotComponent.Style _style;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public AdvancementSlotComponentFactory(
                AdvancementSlotComponent.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
            {
                _style = style;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<SlotKey> Create(SlotKey key)
            {
                return KeyedUiElement<SlotKey>.Wrap(
                    key,
                    new AdvancementSlotComponent(
                        key.Slot, key.AdvancementManager, _style, _uiElementFactory, _iconFactory));
            }
        }

        class AdvancementComponentFactory : IKeyedElementFactory<AdvancementKey>
        {
            private readonly AdvancementComponent.Style _style;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public AdvancementComponentFactory(
                AdvancementComponent.Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
            {
                _style = style;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<AdvancementKey> Create(AdvancementKey key)
            {
                return KeyedUiComponent<AdvancementKey>.Wrap(
                    key, 
                    new DynamicUiCompoundComponent(
                        new StaticAdderController<IAdvancement>(key.Advancement), 
                        AdvancementComponent.Create(
                            key.Advancement,
                            key.AdvancementManager, 
                            new ButtonController(),
                            _style,
                            _uiElementFactory, 
                            _iconFactory)));
            }
        }

        public IUiComponent AdvancementSlots { get; }
        public IUiComponent Advancements { get; }

        private readonly AdvancementRange _range = new();

        public ResearchPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new ResearchPaneController(),
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), "Research"),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            AdvancementSlots = 
                new DynamicUiCompoundComponent(
                    new RadioController<AdvancementSlot>(),
                    DynamicKeyedContainer<SlotKey>.CreateSerial(
                        uiElementFactory.GetClass(s_SlotTable),
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        _range.GetSlots,
                        new AdvancementSlotComponentFactory(s_SlotStyle, uiElementFactory, iconFactory),
                        Comparer<SlotKey>.Create((x, y) => x.Slot.Id.CompareTo(y.Slot.Id))));
            Advancements =
                new DynamicUiCompoundComponent(
                        new AdderComponentController<IAdvancement>(),
                        DynamicKeyedContainer<AdvancementKey>.CreateSerial(
                            uiElementFactory.GetClass(s_AdvancementTable),
                            new TableController(10f),
                            UiSerialContainer.Orientation.Vertical,
                            _range.GetResearchableAdvancements,
                            new AdvancementComponentFactory(s_AdvancementStyle, uiElementFactory, iconFactory),
                            FluentComparator<AdvancementKey>.Comparing(x => x.Advancement.Cost)
                                .Then(x => x.Advancement.Name)));
            var body =
                new DynamicUiSerialContainer(
                    uiElementFactory.GetClass(s_Body),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    AdvancementSlots,
                    Advancements
                };
            SetBody(body);
        }

        public FactionAdvancementManager GetAdvancementManager()
        {
            return _range.GetAdvancementManager();
        }

        public override void Populate(params object?[] args)
        {
            var world = args[0] as World;
            var faction = args[1] as Faction;
            _range.Populate(world, faction);
            Refresh();
            Populated?.Invoke(this, EventArgs.Empty);
        }
    }
}
