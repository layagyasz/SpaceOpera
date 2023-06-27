using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class PoliticsComponentController : IRandomizableFormFieldController<PoliticsGenerator.Parameters>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly Random _random;

        private PoliticsComponent? _component;
        private IRandomizableFormFieldController<int>? _states;
        private IRandomizableFormFieldController<int>? _cultures;

        public PoliticsComponentController(Random random)
        {
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (PoliticsComponent)@object;
            _component.Randomize.Controller.Clicked += HandleRandomize;

            _states = (IRandomizableFormFieldController<int>)_component.States.ComponentController;
            _cultures = (IRandomizableFormFieldController<int>)_component.Cultures.ComponentController;

            _states.ValueChanged += HandleValueChanged;
            _cultures.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component!.Randomize.Controller.Clicked -= HandleRandomize;
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

        public void Randomize(Random random, bool notify = true)
        {
            _states!.Randomize(random, /* notify= */ false);
            _cultures!.Randomize(random, /* notify= */ false);
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
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
