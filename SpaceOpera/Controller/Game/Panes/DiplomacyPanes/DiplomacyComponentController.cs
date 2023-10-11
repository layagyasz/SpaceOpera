using Cardamom.Collections;
using Cardamom.Ui.Controller;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class DiplomacyComponentController : IController
    {
        private DiplomacyComponent? _component;
        private IAdderController<IDiplomaticAgreementSection>? _left;
        private IAdderController<IDiplomaticAgreementSection>? _right;

        private DiplomaticRelation? _relation;
        private DiplomaticAgreement.Builder? _builder;
        private DiplomaticAgreement? _agreement;

        public void Bind(object @object)
        {
            _component = (DiplomacyComponent)@object!;
            _left = (IAdderController<IDiplomaticAgreementSection>)_component.Left.ComponentController;
            _right = (IAdderController<IDiplomaticAgreementSection>)_component.Right.ComponentController;

            _component.Populated += HandlePopulated;
            _left.Added += HandleLeftAdded;
            _right.Added += HandleRightAdded;
        }

        public void Unbind()
        {
            _left!.Added -= HandleLeftAdded;
            _right!.Added -= HandleRightAdded;

            _component = null;
            _left = null;
            _right = null;
        }

        private ISet<DiplomacyType> GetAllowed(bool isLeft)
        {
            var faction = isLeft ? _relation!.Faction : _relation!.Target;
            var range = Enum.GetValues(typeof(DiplomacyType)).Cast<DiplomacyType>().ToEnumSet();
            var blocked = 
                _relation!.CurrentAgreements
                    .Where(x => !_agreement!.Cancels(x))
                    .SelectMany(x => x.GetBlocked(faction))
                    .ToEnumSet();
            var canceled = _agreement!.GetCanceled(faction);
            blocked.UnionWith(canceled);
            blocked.Add(DiplomacyType.Unknown);
            range.ExceptWith(blocked);
            return range;
        }

        private void HandlePopulated(object? sender, DiplomaticRelation relation)
        {
            _relation = relation;
            _builder = new DiplomaticAgreement.Builder().SetProposer(relation.Faction).SetApprover(relation.Target);
            UpdateAgreement();
        }

        private void HandleLeftAdded(object? sender, IDiplomaticAgreementSection section)
        {
            _builder!.AddLeft(section);
            if (section.IsMirrored)
            {
                _builder!.AddRight(section);
            }
            UpdateAgreement();
        }

        private void HandleRightAdded(object? sender, IDiplomaticAgreementSection section)
        {
            _builder!.AddRight(section);
            if (section.IsMirrored)
            {
                _builder!.AddLeft(section);
            }
            UpdateAgreement();
        }
        private void UpdateAgreement()
        {
            _agreement = _builder!.Build();
            _component!.Left.SetRange(GetAllowed(/* isLeft= */ true));
            _component!.Right.SetRange(GetAllowed(/* isLeft= */ false));
            _component!.Agreement.SetAgreement(_agreement!);
        }

    }
}
