using Cardamom.Ui.Controller;
using SpaceOpera.Core.Politics;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class BannerComponentController : IController, IFormElementController<Banner>
    {
        public EventHandler<Banner?>? ValueChanged { get; set; }

        private BannerComponent? _component;
        private IFormElementController<int>? _symbol;
        private IFormElementController<int>? _pattern;
        private IFormElementController<int>? _primaryColor;
        private IFormElementController<int>? _secondaryColor;
        private IFormElementController<int>? _symbolColor;

        public void Bind(object @object)
        {
            _component = (BannerComponent)@object;
            _symbol = (IFormElementController<int>)_component.Symbol.ComponentController;
            _symbol.ValueChanged += HandleValueChanged;
            _pattern = (IFormElementController<int>)_component.Pattern.ComponentController;
            _pattern.ValueChanged += HandleValueChanged;
            _primaryColor = (IFormElementController<int>)_component.PrimaryColor.ComponentController;
            _primaryColor.ValueChanged += HandleValueChanged;
            _secondaryColor = (IFormElementController<int>)_component.SecondaryColor.ComponentController;
            _secondaryColor.ValueChanged += HandleValueChanged;
            _symbolColor = (IFormElementController<int>)_component.SymbolColor.ComponentController;
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

        public Banner? GetValue()
        {
            return new(
                _symbol!.GetValue(),
                _pattern!.GetValue(),
                _primaryColor!.GetValue(),
                _secondaryColor!.GetValue(),
                _symbolColor!.GetValue());
        }

        public void SetValue(Banner? value)
        {
            _symbol!.SetValue(value!.Symbol);
            _pattern!.SetValue(value!.Pattern);
            _primaryColor!.SetValue(value!.PrimaryColor);
            _secondaryColor!.SetValue(value!.SecondaryColor);
            _symbolColor!.SetValue(value!.SymbolColor);
        }

        private void HandleValueChanged(object? sender, int e)
        {
            var value = GetValue();
            _component!.SetBanner(value!);
            ValueChanged?.Invoke(this, value);
        }
    }
}
