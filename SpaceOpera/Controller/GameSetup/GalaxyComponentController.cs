using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.Core.Universe.Generator;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class GalaxyComponentController : IRandomizableFormFieldController<GalaxyGenerator.Parameters>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly Random _random;

        private GalaxyComponent? _component;
        private IRandomizableFormFieldController<float>? _radius;
        private IRandomizableFormFieldController<int>? _shape;
        private IRandomizableFormFieldController<float>? _rotation;
        private IRandomizableFormFieldController<float>? _starDensity;
        private IRandomizableFormFieldController<float>? _transitDensity;

        public GalaxyComponentController(Random random)
        {
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (GalaxyComponent)@object;
            _component.Randomize.Controller.Clicked += HandleRandomize;

            _radius = (IRandomizableFormFieldController<float>)_component.Radius.ComponentController;
            _shape = (IRandomizableFormFieldController<int>)_component.Shape.ComponentController;
            _rotation = (IRandomizableFormFieldController<float>)_component.Rotation.ComponentController;
            _starDensity = (IRandomizableFormFieldController<float>)_component.StarDensity.ComponentController;
            _transitDensity = (IRandomizableFormFieldController<float>)_component.TransitDensity.ComponentController;

            _radius.ValueChanged += HandleValueChanged;
            _shape.ValueChanged += HandleValueChanged;
            _rotation.ValueChanged += HandleValueChanged;
            _starDensity.ValueChanged += HandleValueChanged;
            _transitDensity.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _radius!.ValueChanged -= HandleValueChanged;
            _shape!.ValueChanged -= HandleValueChanged;
            _rotation!.ValueChanged -= HandleValueChanged;
            _starDensity!.ValueChanged -= HandleValueChanged;
            _transitDensity!.ValueChanged -= HandleValueChanged;

            _radius = null;
            _shape = null;
            _rotation = null;
            _starDensity = null;
            _transitDensity = null;

            _component!.Randomize.Controller.Clicked -= HandleRandomize;
            _component = null;
        }

        public GalaxyGenerator.Parameters GetValue()
        {
            return new()
            {
                Radius = _radius!.GetValue(),
                Arms = _shape!.GetValue(),
                Rotation = _rotation!.GetValue(),
                StarDensity = _starDensity!.GetValue(),
                TransitDensity = _transitDensity!.GetValue(),
            };
        }

        public void Randomize(Random random, bool notify = true)
        {
            _radius!.Randomize(random, /* notify= */ true);
            _shape!.Randomize(random, /* notify= */ true);
            _rotation!.Randomize(random, /* notify= */ true);
            _starDensity!.Randomize(random, /* notify= */ true);
            _transitDensity!.Randomize(random, /* notify= */ true);
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetValue(GalaxyGenerator.Parameters value, bool notify = true)
        {
            _radius!.SetValue(value.Radius, /* notify= */ false);
            _shape!.SetValue(value.Arms, /* notify= */ false);
            _rotation!.SetValue(value.Rotation, /* notify= */ false);
            _starDensity!.SetValue(value.StarDensity, /* notify= */ false);
            _transitDensity!.SetValue(value.TransitDensity, /* notify= */ false);
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
