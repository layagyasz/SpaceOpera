using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;
using SpaceOpera.View.Game.Panes.DiplomacyPanes;
using static SpaceOpera.View.Game.Panes.DiplomacyPanes.DiplomaticAgreementOptionsComponent;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class DiplomacyComponentController : IPopupController, IFormFieldController<DiplomaticAgreement>
    {
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }
        public EventHandler<EventArgs>? Submitted { get; set; }
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private DiplomacyComponent? _component;
        private DiplomaticAgreementOptionsComponentController? _addLeft;
        private DiplomaticAgreementOptionsComponentController? _addRight;
        private IAdderController<IDiplomaticAgreementSection>? _removeLeft;
        private IAdderController<IDiplomaticAgreementSection>? _removeRight;

        private World? _world;
        private DiplomaticRelation? _relation;
        private DiplomaticAgreement.Builder? _builder;
        private DiplomaticAgreement? _agreement;

        public void Bind(object @object)
        {
            _component = (DiplomacyComponent)@object!;
            _addLeft = (DiplomaticAgreementOptionsComponentController)_component.LeftOptions.ComponentController;
            _addRight = (DiplomaticAgreementOptionsComponentController)_component.RightOptions.ComponentController;
            _removeLeft = (IAdderController<IDiplomaticAgreementSection>)_component.LeftSections.ComponentController;
            _removeRight = (IAdderController<IDiplomaticAgreementSection>)_component.RightSections.ComponentController;

            _component.Populated += HandlePopulated;
            _addLeft.Added += HandleLeftAdded;
            _addLeft.PopupCreated += HandlePopup;
            _addRight.Added += HandleRightAdded;
            _addRight.PopupCreated += HandlePopup;
            _removeLeft.Added += HandleLeftRemoved;
            _removeRight.Added += HandleRightRemoved;
            _component.Submit.Controller.Clicked += HandleSubmit;
        }

        public void Unbind()
        {
            _addLeft!.Added -= HandleLeftAdded;
            _addLeft!.PopupCreated -= HandlePopup;
            _addRight!.Added -= HandleRightAdded;
            _addRight!.PopupCreated -= HandlePopup;
            _removeLeft!.Added -= HandleLeftRemoved;
            _removeRight!.Added -= HandleRightRemoved;
            _component!.Submit.Controller.Clicked -= HandleSubmit;

            _component = null;
            _addLeft = null;
            _addRight = null;
        }

        public DiplomaticAgreement? GetValue()
        {
            return _agreement;
        }

        public void SetValue(DiplomaticAgreement? value, bool notify)
        {
            _builder = 
                value?.ToBuilder() 
                ?? new DiplomaticAgreement.Builder().SetProposer(_relation!.Faction).SetApprover(_relation!.Target);
            UpdateAgreement();
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private IEnumerable<OptionKey> GetAllowed(bool isLeft)
        {
            var faction = isLeft ? _relation!.Faction : _relation!.Target;
            var agreements = 
                Enumerable.Concat(
                    new List<DiplomaticAgreement>() { _agreement! }, 
                    _relation!.CurrentAgreements.Where(x => !_agreement!.Cancels(x)));
            var currentSet =  
                agreements
                    .Select(x => x.GetTransition(faction))
                    .FirstOrDefault(DiplomaticAgreement.RelationTransition.None);
            var sections = agreements.SelectMany(x => x.GetSections(faction));
            return DiplomacyType.All
                .Where(x => currentSet.SetId < 0 || x.SetId == currentSet.SetId)
                .Where(x => !x.IsUnique || !sections.Any(y => x == y.Type))
                .Select(x => new OptionKey(_world!, faction, x));
        }

        private void HandlePopulated(object? sender, DiplomacyComponent.PopulatedEventArgs e)
        {
            _world = e.World;
            _relation = e.Relation;
            _builder = new DiplomaticAgreement.Builder().SetProposer(_relation.Faction).SetApprover(_relation.Target);
            _component!.LeftOptions.SetRange(Enumerable.Empty<OptionKey>());
            _component!.RightOptions.SetRange(Enumerable.Empty<OptionKey>());
            UpdateAgreement();
        }

        private void HandlePopup(object? sender, PopupEventArgs e)
        {
            PopupCreated?.Invoke(this, e);
        }

        private void HandleLeftAdded(object? sender, IDiplomaticAgreementSection section)
        {
            _builder!.AddLeft(section);
            if (section.Type.IsMirrored)
            {
                _builder!.AddRight(section);
            }
            UpdateAgreement();
        }

        private void HandleRightAdded(object? sender, IDiplomaticAgreementSection section)
        {
            _builder!.AddRight(section);
            if (section.Type.IsMirrored)
            {
                _builder!.AddLeft(section);
            }
            UpdateAgreement();
        }

        private void HandleLeftRemoved(object? sender, IDiplomaticAgreementSection section)
        {
            _builder!.RemoveLeft(section);
            if (section.Type.IsMirrored)
            {
                _builder!.RemoveRight(section);
            }
            UpdateAgreement();
        }

        private void HandleRightRemoved(object? sender, IDiplomaticAgreementSection section)
        {
            _builder!.RemoveRight(section);
            if (section.Type.IsMirrored)
            {
                _builder!.RemoveLeft(section);
            }
            UpdateAgreement();
        }

        private void HandleSubmit(object? sender, MouseButtonClickEventArgs e)
        {
            Submitted?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateAgreement()
        {
            _agreement = _builder!.Build();
            _component!.LeftOptions.SetRange(GetAllowed(/* isLeft= */ true));
            _component!.RightOptions.SetRange(GetAllowed(/* isLeft= */ false));
            _component!.SetAgreement(_agreement!);
        }
    }
}
