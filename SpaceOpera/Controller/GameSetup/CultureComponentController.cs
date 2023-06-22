using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class CultureComponentController : IController, IFormElementController<CulturalTraits>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private CultureComponent? _component;
        private IFormElementController<int>? _ae;
        private IFormElementController<int>? _ic;
        private IFormElementController<int>? _ap;
        private IFormElementController<int>? _cd;
        private IFormElementController<int>? _mh;
        private IFormElementController<int>? _ia;

        public void Bind(object @object)
        {
            _component = (CultureComponent)@object;

            _ae = (IFormElementController<int>)_component.AuthoritarianEgalitarian.ComponentController;
            _ic = (IFormElementController<int>)_component.IndividualistCollectivist.ComponentController;
            _ap = (IFormElementController<int>)_component.AggressivePassive.ComponentController;
            _cd = (IFormElementController<int>)_component.ConventionalDynamic.ComponentController;
            _mh = (IFormElementController<int>)_component.MonumentalHumble.ComponentController;
            _ia = (IFormElementController<int>)_component.IndulgentAustere.ComponentController;

            _ae.ValueChanged += HandleValueChanged;
            _ic.ValueChanged += HandleValueChanged;
            _ap.ValueChanged += HandleValueChanged;
            _cd.ValueChanged += HandleValueChanged;
            _mh.ValueChanged += HandleValueChanged;
            _ia.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component = null;

            _ae!.ValueChanged -= HandleValueChanged;
            _ic!.ValueChanged -= HandleValueChanged;
            _ap!.ValueChanged -= HandleValueChanged;
            _cd!.ValueChanged -= HandleValueChanged;
            _mh!.ValueChanged -= HandleValueChanged;
            _ia!.ValueChanged -= HandleValueChanged;

            _ae = null;
            _ic = null;
            _ap = null;
            _cd = null;
            _mh = null;
            _ia = null;
        }

        public CulturalTraits GetValue()
        {
            return new()
            {
                AuthoritarianEgalitarian = _ae!.GetValue(),
                IndividualistCollectivist = _ic!.GetValue(),
                AggressivePassive = _ap!.GetValue(),
                ConventionalDynamic = _cd!.GetValue(),
                MonumentalHumble = _mh!.GetValue(),
                IndulgentAustere = _ia!.GetValue()
            };
        }

        public void SetValue(CulturalTraits value)
        {
            _ae!.SetValue(value.AuthoritarianEgalitarian);
            _ic!.SetValue(value.IndividualistCollectivist);
            _ap!.SetValue(value.AggressivePassive);
            _cd!.SetValue(value.ConventionalDynamic);
            _mh!.SetValue(value.MonumentalHumble);
            _ia!.SetValue(value.IndulgentAustere);
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
