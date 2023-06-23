using Cardamom.Ui.Controller;
using OpenTK.Mathematics;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class BannerComponentController : IController, IFormElementController<Banner>
    {
        public EventHandler<EventArgs>? ValueChanged { get; set; }

        private BannerComponent? _component;
        private IFormElementController<int>? _symbol;
        private IFormElementController<int>? _pattern;
        private IFormElementController<Color4>? _primaryColor;
        private IFormElementController<Color4>? _secondaryColor;
        private IFormElementController<Color4>? _symbolColor;

        public void Bind(object @object)
        {
            _component = (BannerComponent)@object;
            _symbol = (IFormElementController<int>)_component.Symbol.ComponentController;
            _symbol.ValueChanged += HandleValueChanged;
            _pattern = (IFormElementController<int>)_component.Pattern.ComponentController;
            _pattern.ValueChanged += HandleValueChanged;
            _primaryColor = (IFormElementController<Color4>)_component.PrimaryColor.ComponentController;
            _primaryColor.ValueChanged += HandleValueChanged;
            _secondaryColor = (IFormElementController<Color4>)_component.SecondaryColor.ComponentController;
            _secondaryColor.ValueChanged += HandleValueChanged;
            _symbolColor = (IFormElementController<Color4>)_component.SymbolColor.ComponentController;
            _symbolColor.ValueChanged += HandleValueChanged;

            _component.SetBanner(GetValue()!);
        }

        public void Unbind()
        {
            _symbol!.ValueChanged -= HandleValueChanged;
            _symbol = null;
            _pattern!.ValueChanged -= HandleValueChanged;
            _pattern = null;
            _primaryColor!.ValueChanged -= HandleValueChanged;
            _primaryColor = null;
            _secondaryColor!.ValueChanged -= HandleValueChanged;
            _secondaryColor = null;
            _symbolColor!.ValueChanged -= HandleValueChanged;
            _symbolColor = null;
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

        private void HandleValueChanged(object? sender, EventArgs e)
        {
            var value = GetValue();
            _component!.SetBanner(value!);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
