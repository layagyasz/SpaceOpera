using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Formations;
using SpaceOpera.View.Game.Panes.MilitaryPanes;

namespace SpaceOpera.Controller.Game.Panes.MilitaryPanes
{
    public class FormationsTabController : ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private FormationsTab? _tab;
        private ActionTableController<IFormationDriver>? _formations;
        private ITabController? _summary;

        public void Bind(object @object)
        {
            _tab = (FormationsTab)@object;
            _formations = (ActionTableController<IFormationDriver>)_tab.Formations.ComponentController;
            _summary = (ITabController)_tab.Summary.ComponentController;

            _formations.RowSelected += HandleFormationSelected;
            _formations.Interacted += HandleInteraction;
            _summary.Interacted += HandleInteraction;
            _summary.OrderCreated += HandleOrder;
        }

        public void Unbind()
        {
            _formations!.RowSelected -= HandleFormationSelected;
            _formations!.Interacted -= HandleInteraction;
            _summary!.Interacted -= HandleInteraction;
            _summary!.OrderCreated -= HandleOrder;

            _summary = null;
            _formations = null;
            _tab = null;
        }

        public void Reset() { }

        private void HandleFormationSelected(object? sender, IFormationDriver? driver)
        {
            _tab!.SetSummary(driver);
            _tab.Refresh();
        }

        private void HandleInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.GetOnlyObject() is Type)
            {
                if (_tab!.GetFormationType() == typeof(ArmyDriver))
                {
                    OrderCreated?.Invoke(this, new CreateArmyOrder(_tab!.GetFaction()!));
                }
                else if (_tab!.GetFormationType() == typeof(FleetDriver))
                {
                    OrderCreated?.Invoke(this, new CreateFleetOrder(_tab.GetFaction()!));
                }
            }
            else
            {
                Interacted?.Invoke(this, e);
            }
        }

        private void HandleOrder(object? sender, IOrder order)
        {
            OrderCreated?.Invoke(this, order);
        }
    }
}
