using Cardamom.Ui;
using Cardamom.Ui.Controller;
using SpaceOpera.Core;
using SpaceOpera.Core.Languages.Generator;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Cultures;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.Core.Politics.Governments;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class GovernmentComponentController : IRandomizableFormFieldController<GovernmentParameters>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly FactionGenerator _factionGenerator;
        private readonly LanguageGenerator _languageGenerator;
        private readonly Random _random;

        private GovernmentComponent? _component;
        private IFormFieldController<string>? _name;
        private SelectController<GovernmentForm>? _government;

        private CulturalTraits _culture;
        private NameGenerator? _nameGenerator;

        public GovernmentComponentController(
            FactionGenerator factionGenerator, 
            LanguageGenerator languageGenerator, 
            Random random)
        {
            _factionGenerator = factionGenerator;
            _languageGenerator = languageGenerator;
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (GovernmentComponent)@object;
            _component.Randomize.Controller.Clicked += HandleRandomized;
            _name = (IFormFieldController<string>)_component.Name.Controller;
            _name.ValueChanged += HandleValueChanged;
            _government = (SelectController<GovernmentForm>)_component.Government.ComponentController;
            _government.ValueChanged += HandleValueChanged;
        }

        public void Unbind()
        {
            _component!.Randomize.Controller.Clicked -= HandleRandomized;
            _component = null;
            _name!.ValueChanged -= HandleValueChanged;
            _name = null;
            _government!.ValueChanged -= HandleValueChanged;
            _government = null;
        }

        public GovernmentParameters GetValue()
        {
            return new(_name!.GetValue()!, _nameGenerator!, _government!.GetValue());
        }

        public void Randomize(Random random, bool notify = true)
        {
            var context = new GeneratorContext(null, null, null, random);
            _nameGenerator = 
                new(_languageGenerator.Generate(context), _factionGenerator.ComponentName!.Generate(context));
            _name!.SetValue(_nameGenerator.GenerateNameForFaction(random), /* notify= */ false);
            _government!.Randomize(random, /* notify= */ false);
            if (notify)
            {
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetCulture(CulturalTraits culture)
        {
            _culture = culture;
            _government!.SetRange(
                _factionGenerator.GovernmentForms
                    .Where(x => x.IsValid(culture))
                    .Select(x => SelectOption<GovernmentForm>.Create(x, x.Name)));
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
