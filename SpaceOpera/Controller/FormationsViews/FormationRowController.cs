﻿using Cardamom.Ui;
using SpaceOpera.View;
using SpaceOpera.View.FormationViews;

namespace SpaceOpera.Controller.FormationsViews
{
    public class FormationRowController : IActionController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }

        private FormationRow? _row;

        public void Bind(object @object)
        {
            _row = (FormationRow?)@object;
            _row!.Controller.Clicked += HandleClick;
            foreach (var actionController in _row!.GetActions().Select(x => x.Controller).Cast<IActionController>())
            {
                actionController.Interacted += HandleInteraction;
            }
        }

        public void Unbind()
        {
            _row!.Controller.Clicked -= HandleClick;
            foreach (var actionController in _row!.GetActions().Select(x => x.Controller).Cast<IActionController>())
            {
                actionController.Interacted -= HandleInteraction;
            }
            _row = null;
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e.WithObjects(_row!.GetDrivers()));
        }

        private void HandleClick(object? sender, MouseButtonClickEventArgs e)
        {
            Interacted?.Invoke(this, UiInteractionEventArgs.Create(_row!.GetDrivers(), ActionId.Select));
        }
    }
}
