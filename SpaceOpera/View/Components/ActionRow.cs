using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Components
{
    public class ActionRow<T> : UiCompoundComponent, IKeyedUiElement<T>, IActionRow where T : notnull
    {
        public struct Style
        {
            public string Container { get; set; }
            public string Icon { get; set; }
            public string Text { get; set; }
            public string ActionContainer { get; set; }
        }

        public struct ActionConfiguration
        {
            public string Button { get; set; }
            public ActionId Action { get; set; }
        }

        public EventHandler<EventArgs>? Refreshed { get; set; }

        public T Key { get; }
        private readonly List<IUiElement> _actions = new();

        private ActionRow(Class @class, T key, Icon icon, IUiElement text, IEnumerable<UiWrapper> actions)
            : base(
                  new ActionRowController<T>(key),
                  new UiSerialContainer(@class, new ButtonController(), UiSerialContainer.Orientation.Horizontal))
        {
            Key = key;
            Add(icon);
            Add(text);
            foreach (var action in actions)
            {
                Add(action);
                _actions.AddRange(action);
            }
        }

        public static ActionRow<T> Create(
            T key,
            string name,
            UiElementFactory uiElementFactory,
            IconFactory iconFactory, 
            Style style,
            IEnumerable<ActionConfiguration> actions)
        {
            return new(
                uiElementFactory.GetClass(style.Container),
                key,
                iconFactory.Create(uiElementFactory.GetClass(style.Icon), new InlayController(), key),
                uiElementFactory.CreateTextButton(style.Text, name).Item1,
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

        public void Refresh()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
