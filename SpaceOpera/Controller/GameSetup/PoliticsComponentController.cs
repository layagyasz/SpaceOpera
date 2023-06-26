using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class PoliticsComponentController : IController, IFormElementController<PoliticsGenerator.Parameters>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private PoliticsComponent? _component;
        private IFormElementController<int>? _states;
        private IFormElementController<int>? _cultures;

        public void Bind(object @object)
        {
            _component = (PoliticsComponent)@object;

            _states = (IFormElementController<int>)_component.States.ComponentController;
            _cultures = (IFormElementController<int>)_component.Cultures.ComponentController;

            _states.ValueChanged += HandleValueChanged;
            _cultures.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component = null;

            _states!.ValueChanged -= HandleValueChanged;
            _cultures!.ValueChanged -= HandleValueChanged;

            _states = null;
            _cultures = null;
        }

        public PoliticsGenerator.Parameters GetValue()
        {
            return new()
            {
                States = _states!.GetValue(),
                Cultures = _cultures!.GetValue(),
            };
        }

        public void SetValue(PoliticsGenerator.Parameters value, bool notify = true)
        {
            _states!.SetValue(value.States, /* notify= */ false);
            _cultures!.SetValue(value.Cultures, /* notify= */ false);
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
