using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.GameSetup
{
    public class BannerComponent : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? Banner { get; set; }
            public DialSelect.Style? Symbol { get; set; }
            public DialSelect.Style? Pattern { get; set; }
            public DialSelect.Style? PrimaryColor { get; set; }
            public DialSelect.Style? SecondaryColor { get; set; }
            public DialSelect.Style? SymbolColor { get; set; }
        }

        public IUiComponent Symbol { get; }
        public IUiComponent Pattern { get; }
        public IUiComponent PrimaryColor { get; }
        public IUiComponent SecondaryColor { get; }
        public IUiComponent SymbolColor { get; }

        private readonly Class _iconClass;
        private readonly IconFactory _iconFactory;

        private Icon? _icon;

        private BannerComponent(
            Class @class, 
            IController controller, 
            IUiComponent symbol,
            IUiComponent pattern,
            IUiComponent primaryColor,
            IUiComponent secondaryColor,
            IUiComponent symbolColor,
            Class iconClass,
            IconFactory iconFactory)
            : base(
                  controller,
                  new UiSerialContainer(
                      @class, new NoOpElementController<UiSerialContainer>(), UiSerialContainer.Orientation.Vertical))
        {
            Symbol = symbol;
            Pattern = pattern;
            PrimaryColor = primaryColor;
            SecondaryColor = secondaryColor;
            SymbolColor = symbolColor;

            _iconClass = iconClass;
            _iconFactory = iconFactory;

            Add(Symbol);
            Add(Pattern);
            Add(PrimaryColor);
            Add(SecondaryColor);
            Add(SymbolColor);
        }

        public void SetBanner(Banner banner)
        {
            if (_icon != null)
            {
                Remove(_icon);
                _icon.Dispose();
            }
            _icon = _iconFactory.Create(_iconClass, new InlayController(), banner);
            _icon.Initialize();
            Insert(0, _icon);
        }

        public static BannerComponent Create(
            UiElementFactory uiElementFactory, IconFactory iconFactory, Style style, BannerGenerator bannerGenerator)
        {
            var symbol =
                DialSelect.Create(
                    uiElementFactory,
                    style.Symbol!,
                    Enumerable.Range(0, bannerGenerator.Symbols)
                        .Select(x => SelectOption<int>.Create(x, $"Symbol {x + 1}")),
                    /* wrap= */ true);
            var pattern =
                DialSelect.Create(
                    uiElementFactory,
                    style.Pattern!,
                    Enumerable.Range(0, bannerGenerator.Patterns)
                        .Select(x => SelectOption<int>.Create(x, $"Pattern {x + 1}")),
                    /* wrap= */ true);
            var colorOptions = 
                Enumerable.Range(0, bannerGenerator.Colors)
                    .Select(x => SelectOption<int>.Create(x, $"Color {x + 1}"))
                    .ToList();
            var primaryColor = 
                DialSelect.Create(uiElementFactory, style.PrimaryColor!, colorOptions, /* wrap= */ true);
            var secondaryColor =
                DialSelect.Create(uiElementFactory, style.SecondaryColor!, colorOptions, /* wrap= */ true);
            var symbolColor =
                DialSelect.Create(uiElementFactory, style.SymbolColor!, colorOptions, /* wrap= */ true);
            return new(
                uiElementFactory.GetClass(style.Container!), 
                new BannerComponentController(),
                symbol, 
                pattern,
                primaryColor, 
                secondaryColor,
                symbolColor, 
                uiElementFactory.GetClass(style.Banner!), 
                iconFactory);      
        }
    } 
}
