using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class CultureComponentController : IController, IFormElementController<CulturalTraits>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private CultureComponent? _component;
        private IFormElementController<int>? _aeController;
        private IFormElementController<int>? _icController;
        private IFormElementController<int>? _apController;
        private IFormElementController<int>? _cdController;
        private IFormElementController<int>? _mhController;
        private IFormElementController<int>? _iaController;

        public void Bind(object @object)
        {
            _component = (CultureComponent)@object;

            _aeController = (IFormElementController<int>)_component.AuthoritarianEgalitarian.ComponentController;
            _icController = (IFormElementController<int>)_component.IndividualistCollectivist.ComponentController;
            _apController = (IFormElementController<int>)_component.AggressivePassive.ComponentController;
            _cdController = (IFormElementController<int>)_component.ConventionalDynamic.ComponentController;
            _mhController = (IFormElementController<int>)_component.MonumentalHumble.ComponentController;
            _iaController = (IFormElementController<int>)_component.IndulgentAustere.ComponentController;

            _aeController.ValueChanged += HandleValueChanged;
            _icController.ValueChanged += HandleValueChanged;
            _apController.ValueChanged += HandleValueChanged;
            _cdController.ValueChanged += HandleValueChanged;
            _mhController.ValueChanged += HandleValueChanged;
            _iaController.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component = null;

            _aeController!.ValueChanged -= HandleValueChanged;
            _icController!.ValueChanged -= HandleValueChanged;
            _apController!.ValueChanged -= HandleValueChanged;
            _cdController!.ValueChanged -= HandleValueChanged;
            _mhController!.ValueChanged -= HandleValueChanged;
            _iaController!.ValueChanged -= HandleValueChanged;

            _aeController = null;
            _icController = null;
            _apController = null;
            _cdController = null;
            _mhController = null;
            _iaController = null;
        }

        public CulturalTraits GetValue()
        {
            return new()
            {
                AuthoritarianEgalitarian = _aeController!.GetValue(),
                IndividualistCollectivist = _icController!.GetValue(),
                AggressivePassive = _apController!.GetValue(),
                ConventionalDynamic = _cdController!.GetValue(),
                MonumentalHumble = _mhController!.GetValue(),
                IndulgentAustere = _iaController!.GetValue()
            };
        }

        public void SetValue(CulturalTraits value)
        {
            _aeController!.SetValue(value.AuthoritarianEgalitarian);
            _icController!.SetValue(value.IndividualistCollectivist);
            _apController!.SetValue(value.AggressivePassive);
            _cdController!.SetValue(value.ConventionalDynamic);
            _mhController!.SetValue(value.MonumentalHumble);
            _iaController!.SetValue(value.IndulgentAustere);
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
