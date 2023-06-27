using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class CultureComponentController : IRandomizableFormFieldController<CulturalTraits>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly Random _random;

        private CultureComponent? _component;
        private IRandomizableFormFieldController<int>? _ae;
        private IRandomizableFormFieldController<int>? _ic;
        private IRandomizableFormFieldController<int>? _ap;
        private IRandomizableFormFieldController<int>? _cd;
        private IRandomizableFormFieldController<int>? _mh;
        private IRandomizableFormFieldController<int>? _ia;

        public CultureComponentController(Random random)
        {
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (CultureComponent)@object;
            _component.Randomize.Controller.Clicked += HandleRandomize;

            _ae = (IRandomizableFormFieldController<int>)_component.AuthoritarianEgalitarian.ComponentController;
            _ic = (IRandomizableFormFieldController<int>)_component.IndividualistCollectivist.ComponentController;
            _ap = (IRandomizableFormFieldController<int>)_component.AggressivePassive.ComponentController;
            _cd = (IRandomizableFormFieldController<int>)_component.ConventionalDynamic.ComponentController;
            _mh = (IRandomizableFormFieldController<int>)_component.MonumentalHumble.ComponentController;
            _ia = (IRandomizableFormFieldController<int>)_component.IndulgentAustere.ComponentController;

            _ae.ValueChanged += HandleValueChanged;
            _ic.ValueChanged += HandleValueChanged;
            _ap.ValueChanged += HandleValueChanged;
            _cd.ValueChanged += HandleValueChanged;
            _mh.ValueChanged += HandleValueChanged;
            _ia.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component!.Randomize.Controller.Clicked -= HandleRandomize;
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

        public void Randomize(Random random, bool notify = true)
        {
            _ae!.Randomize(random, /* notify= */ false);
            _ic!.Randomize(random, /* notify= */ false);
            _ap!.Randomize(random, /* notify= */ false);
            _cd!.Randomize(random, /* notify= */ false);
            _mh!.Randomize(random, /* notify= */ false);
            _ia!.Randomize(random, /* notify= */ false);
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetValue(CulturalTraits value, bool notify = true)
        {
            _ae!.SetValue(value.AuthoritarianEgalitarian, /* notify= */ false);
            _ic!.SetValue(value.IndividualistCollectivist, /* notify= */ false);
            _ap!.SetValue(value.AggressivePassive, /* notify= */ false);
            _cd!.SetValue(value.ConventionalDynamic, /* notify= */ false);
            _mh!.SetValue(value.MonumentalHumble, /* notify= */ false);
            _ia!.SetValue(value.IndulgentAustere, /* notify= */ false);
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleRandomize(object? sender, MouseButtonClickEventArgs e)
        {
            Randomize(_random, /* notify= */ true);
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
