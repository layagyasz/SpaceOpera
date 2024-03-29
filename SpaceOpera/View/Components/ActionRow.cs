﻿using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Components
{
    public class ActionRow<T> : DynamicUiCompoundComponent, IKeyedUiElement<T>, IActionRow where T : notnull
    {
        public EventHandler<ElementEventArgs>? ActionAdded { get; set; }
        public EventHandler<ElementEventArgs>? ActionRemoved { get; set; }

        public T Key { get; }
        private readonly List<IUiElement> _actions = new();

        private ActionRow(
            Class @class, 
            T key, 
            ActionId clickAction, 
            ActionId rightClickAction,
            IEnumerable<IUiElement> info,
            IEnumerable<UiWrapper> actions)
            : base(
                  new ActionRowController<T>(key, clickAction, rightClickAction),
                  new DynamicUiSerialContainer(
                      @class, new OptionElementController<T>(key), UiSerialContainer.Orientation.Horizontal))
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
            ActionId rightClickAction,
            UiElementFactory uiElementFactory,
            ActionRowStyles.Style style,
            IEnumerable<IUiElement> info,
            IEnumerable<ActionRowStyles.ActionConfiguration> actions)
        {
            return new(
                uiElementFactory.GetClass(style.Container!),
                key,
                clickAction,
                rightClickAction,
                info,
                actions.Select(
                    x =>
                        new UiWrapper(
                            uiElementFactory.GetClass(style.ActionContainer!),
                            new ButtonController(),
                            new SimpleUiElement(
                                uiElementFactory.GetClass(x.Button!),
                                new ActionButtonController(x.Action)))));
        }

        public IEnumerable<IUiElement> GetActions()
        {
            return _actions;
        }
    }
}
