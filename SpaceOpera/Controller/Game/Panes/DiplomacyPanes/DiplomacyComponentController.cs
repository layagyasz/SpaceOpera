using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class DiplomacyComponentController : IController
    {
        private DiplomacyComponent? _component;
        private IActionController? _left;
        private IActionController? _right;

        private DiplomaticAgreement.Builder _agreement = new();

        public void Bind(object @object)
        {
            _component = (DiplomacyComponent)@object!;
            _left = (IActionController)_component.Left.ComponentController;
            _right = (IActionController)_component.Right.ComponentController;

            _component.Populated += HandlePopulated;
            _left.Interacted += HandleLeftAction;
            _right.Interacted += HandleRightAction;
        }

        public void Unbind()
        {
            _left!.Interacted -= HandleLeftAction;
            _right!.Interacted -= HandleRightAction;

            _component = null;
            _left = null;
            _right = null;
        }

        private void HandlePopulated(object? sender, DiplomaticRelation relation)
        {
            _agreement = new DiplomaticAgreement.Builder().SetProposer(relation.Faction).SetApprover(relation.Target);
            UpdateAgreement();
        }

        private void HandleLeftAction(object? sender, UiInteractionEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleRightAction(object? sender, UiInteractionEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void UpdateAgreement()
        {
            _component!.Agreement.SetAgreement(_agreement.Build());
        }

    }
}
