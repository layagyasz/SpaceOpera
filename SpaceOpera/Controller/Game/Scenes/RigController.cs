﻿using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Window;
using OpenTK.Windowing.Common;

namespace SpaceOpera.Controller.Game.Scenes
{
    public class RigController : IActionController, IElementController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<MouseButtonClickEventArgs>? Clicked { get; set; }
        public EventHandler<EventArgs>? Focused { get; set; }
        public EventHandler<EventArgs>? FocusLeft { get; set; }
        public EventHandler<MouseButtonDragEventArgs>? MouseDragged { get; set; }
        public EventHandler<EventArgs>? MouseEntered { get; set; }
        public EventHandler<EventArgs>? MouseLeft { get; set; }


        private readonly IActionController[] _subControllers;

        public RigController(params IActionController[] subControllers)
        {
            _subControllers = subControllers;
        }


        public void Bind(object @object)
        {
            foreach (var subController in _subControllers)
            {
                subController.Interacted += HandleInteraction;
            }
        }

        public void Unbind()
        {
            foreach (var subController in _subControllers)
            {
                subController.Interacted -= HandleInteraction;
            }
        }

        public virtual bool HandleKeyDown(KeyDownEventArgs e)
        {
            return false;
        }

        public virtual bool HandleTextEntered(TextEnteredEventArgs e)
        {
            return false;
        }

        public bool HandleMouseEntered()
        {
            return false;
        }

        public bool HandleMouseLeft()
        {
            return false;
        }

        public bool HandleMouseButtonClicked(MouseButtonClickEventArgs e)
        {
            return false;
        }

        public virtual bool HandleMouseButtonDragged(MouseButtonDragEventArgs e)
        {
            return false;
        }

        public virtual bool HandleMouseWheelScrolled(MouseWheelEventArgs e)
        {
            return false;
        }

        public bool HandleMouseLingered()
        {
            return false;
        }

        public bool HandleMouseLingerBroken()
        {
            return false;
        }

        public virtual bool HandleFocusEntered()
        {
            return false;
        }

        public virtual bool HandleFocusLeft()
        {
            return false;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
