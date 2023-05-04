using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Components
{
    public class ActionRow<T> : DynamicUiCompoundComponent, IKeyedUiElement<T>, IActionRow where T : notnull
    {
        public struct Style
        {
            public string Container { get; set; }
            public string ActionContainer { get; set; }
        }

        public struct ActionConfiguration
        {
            public string Button { get; set; }
            public ActionId Action { get; set; }
        }

        public EventHandler<ElementEventArgs>? ActionAdded { get; set; }
        public EventHandler<ElementEventArgs>? ActionRemoved { get; set; }

        public T Key { get; }
        private readonly List<IUiElement> _actions = new();

        private ActionRow(
            Class @class, T key, ActionId clickAction, IEnumerable<IUiElement> info, IEnumerable<UiWrapper> actions)
            : base(
                  new ActionRowController<T>(key, clickAction),
                  new DynamicUiSerialContainer(
                      @class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            Key = key;
            foreach (var i in info)
            {
                Add(i);
            }
            foreach (var action in actions)
            {
                Add(action);
                _actions.AddRange(action);
            }
        }

        public static ActionRow<T> Create(
            T key,
            ActionId clickAction,
            UiElementFactory uiElementFactory,
            Style style,
            IEnumerable<IUiElement> info,
            IEnumerable<ActionConfiguration> actions)
        {
            return new(
                uiElementFactory.GetClass(style.Container),
                key,
                clickAction,
                info,
                actions.Select(
                    x =>
                        new UiWrapper(
                            uiElementFactory.GetClass(style.ActionContainer),
                            new ButtonController(),
                            new SimpleUiElement(
                                uiElementFactory.GetClass(x.Button),
                                new ActionButtonController(x.Action)))));
        }

        public IEnumerable<IUiElement> GetActions()
        {
            return _actions;
        }
    }
}
