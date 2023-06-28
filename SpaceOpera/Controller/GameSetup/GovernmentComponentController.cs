using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.Core;
using SpaceOpera.Core.Languages.Generator;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class GovernmentComponentController : IRandomizableFormFieldController<GovernmentParameters>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly LanguageGenerator _languageGenerator;
        private readonly ComponentNameGeneratorGenerator _nameGeneratorGenerator;
        private readonly Random _random;

        private GovernmentComponent? _component;
        private IFormFieldController<string>? _name;

        private NameGenerator? _nameGenerator;

        public GovernmentComponentController(
            LanguageGenerator languageGenerator, ComponentNameGeneratorGenerator nameGeneratorGenerator, Random random)
        {
            _languageGenerator = languageGenerator;
            _nameGeneratorGenerator = nameGeneratorGenerator;
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (GovernmentComponent)@object;
            _component.Randomize.Controller.Clicked += HandleRandomized;
            _name = (IFormFieldController<string>)_component.Name.Controller;
            _name.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component!.Randomize.Controller.Clicked -= HandleRandomized;
            _component = null;
            _name!.ValueChanged -= HandleValueChanged;
            _name = null;
        }

        public GovernmentParameters GetValue()
        {
            return GovernmentParameters.Create(_name!.GetValue()!, _nameGenerator!);
        }

        public void Randomize(Random random, bool notify = true)
        {
            var context = new GeneratorContext(null, null, random);
            _nameGenerator = new(_languageGenerator.Generate(context), _nameGeneratorGenerator.Generate(context));
            _name!.SetValue(_nameGenerator.GenerateNameForFaction(random), notify);
        }

        public void SetValue(GovernmentParameters? value, bool notify)
        {
            _nameGenerator = value?.NameGenerator;
            _name!.SetValue(value?.Name, notify);
        }

        private void HandleRandomized(object? sender, MouseButtonClickEventArgs e)
        {
            Randomize(_random);
        }

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
