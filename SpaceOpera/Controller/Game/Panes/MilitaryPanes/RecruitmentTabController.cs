using Cardamom.Ui.Controller;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Economics;
using SpaceOpera.Core.Orders.Formations;
using SpaceOpera.View;
using SpaceOpera.View.Game.Panes.MilitaryPanes;

namespace SpaceOpera.Controller.Game.Panes.MilitaryPanes
{
    public class RecruitmentTabController : ITabController
    {
        public EventHandler<UiInteractionEventArgs>? Interacted { get; set; }
        public EventHandler<IOrder>? OrderCreated { get; set; }

        private RecruitmentTab? _tab;
        private SelectController<EconomicSubzoneHolding>? _holding;
        private IActionController? _templates;
        private IActionController? _projects;

        public void Bind(object @object)
        {
            _tab = (RecruitmentTab)@object;
            _holding = (SelectController<EconomicSubzoneHolding>)_tab.Holding.ComponentController;
            _templates = (IActionController)_tab.Templates.ComponentController;
            _projects = (IActionController)_tab.Projects.ComponentController;

            _tab.Populated += HandlePopulated;
            _templates.Interacted += HandleTemplateInteraction;
            _projects.Interacted += HandleProjectInteraction;
        }

        public void Unbind()
        {
            _tab!.Populated -= HandlePopulated;
            _templates!.Interacted -= HandleTemplateInteraction;
            _projects!.Interacted -= HandleProjectInteraction;

            _projects = null;
            _templates = null;
            _holding = null;
            _tab = null;
        }

        public void Reset() { }

        private void HandlePopulated(object? sender, EventArgs e)
        {
            _holding!.SetRange(
                _tab!.GetWorld().Economy.GetHolding(_tab.GetFaction()).GetHoldings()
                    .SelectMany(x => x.GetHoldings())
                    .Select(x => SelectOption<EconomicSubzoneHolding>.Create(x, $"{x.Parent.Name} - {x.Name}")));
        }

        private void HandleProjectInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.GetOnlyObject() is IProject project)
            {
                if (e.Action == ActionId.Cancel)
                {
                    OrderCreated?.Invoke(this, new CancelProjectOrder(project));
                }
            }
        }

        private void HandleTemplateInteraction(object? sender, UiInteractionEventArgs e)
        {
            if (e.GetOnlyObject() is DivisionTemplate template)
            {
                if (e.Action == ActionId.Add)
                {
                    if (_holding!.GetValue() != null)
                    {
                        OrderCreated?.Invoke(this, new CreateDivisionOrder(_holding.GetValue()!, template));
                    }
                }
            }
        }
    }
}
