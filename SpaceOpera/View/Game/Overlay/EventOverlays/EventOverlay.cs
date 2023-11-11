using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Components;
using SpaceOpera.Core.Events;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Overlay.EventOverlays
{
    public class EventOverlay : DynamicUiCompoundComponent, IOverlay
    {
        private static readonly string s_Container = "event-overlay-container";
        private static readonly string s_List = "event-overlay-list";

        private static readonly ActionRow<IEvent>.Style s_CellStyle =
            new()
            {
                Container = "event-overlay-event-container"
            };

        class EventRange
        {
            public World? World { get; set; }
            public Faction? Faction { get; set; }

            public IEnumerable<IEvent> GetRange()
            {
                if (World == null || Faction == null)
                {
                    return Enumerable.Empty<IEvent>();
                }
                return World.Events.Get(Faction);
            }
        }

        private readonly EventRange _range = new();

        public EventOverlay(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                new ActionComponentController(),
                new DynamicUiSerialContainer(
                  uiElementFactory.GetClass(s_Container),
                  new NoOpElementController(),
                  UiSerialContainer.Orientation.Vertical))
        {
            Add(
                new DynamicUiCompoundComponent(
                    new ActionComponentController(),
                    DynamicKeyedContainer<IEvent>.CreateSerial(
                        uiElementFactory.GetClass(s_List),
                        new NoOpElementController(),
                        UiSerialContainer.Orientation.Horizontal,
                        _range.GetRange,
                        new SimpleKeyedElementFactory<IEvent>(uiElementFactory, iconFactory, CreateCell),
                        Comparer<IEvent>.Create((x, y) => 0))));
            Position = new(0, 68, 0);
        }

        public void Populate(params object?[] args)
        {
            _range.World = (World?)args[0];
            _range.Faction = (Faction?)args[1];
            Refresh();
        }

        private static IKeyedUiElement<IEvent> CreateCell(
            IEvent @event, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            return ActionRow<IEvent>.Create(
                @event,
                ActionId.Select,
                ActionId.Ignore,
                uiElementFactory,
                s_CellStyle,
                new List<IUiElement>()
                {
                    new SimpleUiElement(uiElementFactory.GetClass(GetClass(@event)), new InlayController())
                },
                Enumerable.Empty<ActionRow<IEvent>.ActionConfiguration>());
        }

        private static string GetClass(IEvent @event)
        {
            if (@event is DiplomaticStatusChangeNotification diplomaticStatusChange)
            {
                switch (diplomaticStatusChange.Status)
                {
                    case DiplomaticRelation.DiplomaticStatus.Peace:
                        return "event-overlay-event-peace";
                    case DiplomaticRelation.DiplomaticStatus.War:
                        return "event-overlay-event-war";
                }
            }
            if (@event is FormationDestroyedEvent)
            {
                return "event-overlay-event-formation-destroyed";
            }

            throw new ArgumentException($"Unsupported event {@event}");
        }
    }
}
