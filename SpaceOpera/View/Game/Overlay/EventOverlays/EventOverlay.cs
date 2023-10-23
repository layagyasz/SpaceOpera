using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core;
using SpaceOpera.View.Components;
using SpaceOpera.Core.Events;

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

        private readonly UiElementFactory _uiElementFactory;

        private World? _world;
        private Faction? _faction;

        public EventOverlay(UiElementFactory uiElementFactory)
            : base(
                new ActionComponentController(),
                new DynamicUiSerialContainer(
                  uiElementFactory.GetClass(s_Container),
                  new NoOpElementController(),
                  UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory;

            Add(
                new DynamicKeyedTable<IEvent, ActionRow<IEvent>>(
                    uiElementFactory.GetClass(s_List),
                    new NoOpElementController(),
                    UiSerialContainer.Orientation.Horizontal,
                    GetRange,
                    CreateCell,
                    Comparer<IEvent>.Create((x, y) => 0)));
            Position = new(0, 68, 0);
        }

        public void Populate(params object?[] args)
        {
            _world = (World?)args[0];
            _faction = (Faction?)args[1];
            Refresh();
        }

        private ActionRow<IEvent> CreateCell(IEvent @event)
        {
            return ActionRow<IEvent>.Create(
                @event, 
                ActionId.Select, 
                _uiElementFactory,
                s_CellStyle,
                new List<IUiElement>() 
                { 
                    new SimpleUiElement(_uiElementFactory.GetClass(GetClass(@event)), new InlayController())
                },
                Enumerable.Empty<ActionRow<IEvent>.ActionConfiguration>());
        }

        private IEnumerable<IEvent> GetRange()
        {
            if (_world == null || _faction == null)
            {
                return Enumerable.Empty<IEvent>();
            }
            return _world.EventManager.Get(_faction);
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

            throw new ArgumentException($"Unsupported event {@event}");
        }
    }
}
