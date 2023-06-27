using Cardamom.Ui;
using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class BannerComponentController : IRandomizableFormFieldController<Banner>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private readonly BannerGenerator _bannerGenerator;
        private readonly Random _random;

        private BannerComponent? _component;
        private IFormFieldController<int>? _symbol;
        private IFormFieldController<int>? _pattern;
        private IFormFieldController<Color4>? _primaryColor;
        private IFormFieldController<Color4>? _secondaryColor;
        private IFormFieldController<Color4>? _symbolColor;

        public BannerComponentController(BannerGenerator bannerGenerator, Random random)
        {
            _bannerGenerator = bannerGenerator;
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (BannerComponent)@object;
            _component.Randomize.Controller.Clicked += HandleRandomize;

            _symbol = (IFormFieldController<int>)_component.Symbol.ComponentController;
            _pattern = (IFormFieldController<int>)_component.Pattern.ComponentController;
            _primaryColor = (IFormFieldController<Color4>)_component.PrimaryColor.ComponentController;
            _secondaryColor = (IFormFieldController<Color4>)_component.SecondaryColor.ComponentController;
            _symbolColor = (IFormFieldController<Color4>)_component.SymbolColor.ComponentController;

            _symbol.ValueChanged += HandleValueChanged;
            _pattern.ValueChanged += HandleValueChanged;
            _primaryColor.ValueChanged += HandleValueChanged;
            _secondaryColor.ValueChanged += HandleValueChanged;
            _symbolColor.ValueChanged += HandleValueChanged;

            _component.SetBanner(GetValue()!);
        }

        public void Unbind()
        {
            _symbol!.ValueChanged -= HandleValueChanged;
            _pattern!.ValueChanged -= HandleValueChanged;
            _primaryColor!.ValueChanged -= HandleValueChanged;
            _secondaryColor!.ValueChanged -= HandleValueChanged;
            _symbolColor!.ValueChanged -= HandleValueChanged;

            _symbol = null;
            _pattern = null;
            _primaryColor = null;
            _secondaryColor = null;
            _symbolColor = null;

            _component!.Randomize.Controller.Clicked -= HandleRandomize;
            _component = null;
        }

        public Banner GetValue()
        {
            return new(
                _symbol!.GetValue(),
                _pattern!.GetValue(),
                _primaryColor!.GetValue(),
                _secondaryColor!.GetValue(),
                _symbolColor!.GetValue());
        }

        public void Randomize(Random random, bool notify)
        {
            SetValue(_bannerGenerator.Generate(new(null, null, random)), notify);
        }

        public void SetValue(Banner? value, bool notify = true)
        {
            _symbol!.SetValue(value!.Symbol, /* notify= */ false);
            _pattern!.SetValue(value!.Pattern, /* notify= */ false);
            _primaryColor!.SetValue(value!.PrimaryColor, /* notify= */ false);
            _secondaryColor!.SetValue(value!.SecondaryColor, /* notify= */ false);
            _symbolColor!.SetValue(value!.SymbolColor, /* notify= */ false);
            _component!.SetBanner(GetValue()!);
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
            var value = GetValue();
            _component!.SetBanner(value!);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
