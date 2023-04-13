﻿using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Panes.FormationPanes;

namespace SpaceOpera.Controller.Panes.FormationPanes
{
    public class FormationPaneController : PaneController, IGamePaneController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        protected FormationPane? _pane;

        public override void Bind(object @object)
        {
            base.Bind(@object);
            _pane = (FormationPane)@object;
            var listController = (FormationListController)_pane!.FormationList.ComponentController;
            listController.Closed += HandleClose;
            listController.Interacted += HandleInteraction;
        }

        public override void Unbind()
        {
            var listController = (FormationListController)_pane!.FormationList.ComponentController;
            listController.Closed -= HandleClose;
            listController.Interacted -= HandleInteraction;
            base.Unbind();
        }

        private void HandleClose(object? sender, EventArgs e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            Interacted?.Invoke(this, e);
        }
    }
}
