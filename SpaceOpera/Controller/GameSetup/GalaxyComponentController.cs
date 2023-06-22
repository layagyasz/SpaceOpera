using Cardamom.Ui.Controller;
using SpaceOpera.Core.Universe.Generator;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class GalaxyComponentController : IController, IFormElementController<GalaxyGenerator.Parameters>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private GalaxyComponent? _component;
        private IFormElementController<float>? _radius;
        private IFormElementController<int>? _shape;
        private IFormElementController<float>? _rotation;
        private IFormElementController<float>? _starDensity;
        private IFormElementController<float>? _transitDensity;

        public void Bind(object @object)
        {
            _component = (GalaxyComponent)@object;

            _radius = (IFormElementController<float>)_component.Radius.ComponentController;
            _shape = (IFormElementController<int>)_component.Shape.ComponentController;
            _rotation = (IFormElementController<float>)_component.Rotation.ComponentController;
            _starDensity = (IFormElementController<float>)_component.StarDensity.ComponentController;
            _transitDensity = (IFormElementController<float>)_component.TransitDensity.ComponentController;

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

        public void SetValue(GalaxyGenerator.Parameters value)
        {
            _radius!.SetValue(value.Radius);
            _shape!.SetValue(value.Arms);
            _rotation!.SetValue(value.Rotation);
            _starDensity!.SetValue(value.StarDensity);
            _transitDensity!.SetValue(value.TransitDensity);
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
