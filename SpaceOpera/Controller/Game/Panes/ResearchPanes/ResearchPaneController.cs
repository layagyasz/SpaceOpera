using Cardamom.Ui.Controller;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Panes.ResearchPanes;

namespace SpaceOpera.Controller.Game.Panes.ResearchPanes
{
    public class ResearchPaneController : GamePaneController
    {
        private FactionAdvancementManager? _advancementManager;
        private RadioController<AdvancementSlot>? _slot;
        private IAdderController<IAdvancement>? _advancement;

        public override void Bind(object @object)
        {
            var pane = (ResearchPane)@object;
            _slot = (RadioController<AdvancementSlot>)pane.AdvancementSlots.ComponentController;
            _advancement = (IAdderController<IAdvancement>)pane.Advancements.ComponentController;
            pane.Populated += HandlePopulated;
            _advancement.Added += HandleAdvancementSelected;
            base.Bind(@object);
        }

        public override void Unbind()
        {
            _pane!.Populated -= HandlePopulated;
            _advancement!.Added -= HandleAdvancementSelected;
            _slot = null;
            _advancement = null;
            base.Unbind();
        }

        private void HandleAdvancementSelected(object? sender, IAdvancement advancement)
        {
            var slot = _slot!.GetValue();
            if (slot != null)
            {
                OrderCreated?.Invoke(this, new SetResearchOrder(_advancementManager!, slot, advancement));
            }
        }

        private void HandlePopulated(object? sender, EventArgs e)
        {
            var pane = (ResearchPane)_pane!;
            _advancementManager = pane.GetAdvancementManager();
        }
    }
}
