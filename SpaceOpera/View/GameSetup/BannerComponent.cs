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
            public string? SectionHeaderContainer { get; set; }
            public string? SectionHeader { get; set; }
            public string? Randomize { get; set; }
            public string? FieldHeader { get; set; }
            public string? Banner { get; set; }
            public DialSelect.Style? Symbol { get; set; }
            public DialSelect.Style? Pattern { get; set; }
            public ColorSelect.Style? PrimaryColor { get; set; }
            public ColorSelect.Style? SecondaryColor { get; set; }
            public ColorSelect.Style? SymbolColor { get; set; }
        }

        public IUiElement Randomize { get; }
        public IUiComponent Symbol { get; }
        public IUiComponent Pattern { get; }
        public IUiComponent PrimaryColor { get; }
        public IUiComponent SecondaryColor { get; }
        public IUiComponent SymbolColor { get; }

        private readonly Class _iconClass;
        private readonly IconFactory _iconFactory;

        private Icon? _icon;

        public BannerComponent(
            UiElementFactory uiElementFactory,
            IconFactory iconFactory,
            Style style, 
            BannerGenerator bannerGenerator,
            Random random)
            : base(
                  new BannerComponentController(bannerGenerator, random),
                  new UiSerialContainer(
                      uiElementFactory.GetClass(style.Container!), 
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Randomize = new SimpleUiElement(uiElementFactory.GetClass(style.Randomize!), new ButtonController());
            Symbol =
                DialSelect.Create(
                    uiElementFactory,
                    style.Symbol!,
                    Enumerable.Range(0, bannerGenerator.Symbols)
                        .Select(x => SelectOption<int>.Create(x, $"Symbol {x + 1}")),
                    0,
                    /* wrap= */ true);
            Pattern =
                DialSelect.Create(
                    uiElementFactory,
                    style.Pattern!,
                    Enumerable.Range(0, bannerGenerator.Patterns)
                        .Select(x => SelectOption<int>.Create(x, $"Pattern {x + 1}")),
                    0,
                    /* wrap= */ true);
            PrimaryColor = ColorSelect.Create(uiElementFactory, style.PrimaryColor!, bannerGenerator.Colors);
            SecondaryColor = ColorSelect.Create(uiElementFactory, style.SecondaryColor!, bannerGenerator.Colors);
            SymbolColor = ColorSelect.Create(uiElementFactory, style.SymbolColor!, bannerGenerator.Colors);

            _iconClass = uiElementFactory.GetClass(style.Banner!);
            _iconFactory = iconFactory;

            Add(
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.SectionHeaderContainer!), 
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Horizontal)
                {
                    new TextUiElement(
                        uiElementFactory.GetClass(style.SectionHeader!), new ButtonController(), "Banner"),
                    Randomize
                });
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Symbol"));
            Add(Symbol);
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Pattern"));
            Add(Pattern);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Primary Color"));
            Add(PrimaryColor);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Secondary Color"));
            Add(SecondaryColor);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Symbol Color"));
            Add(SymbolColor);
        }

        public void SetBanner(Banner banner)
        {
            if (_icon != null)
            {
                Remove(_icon);
                _icon.Dispose();
            }
            _icon = _iconFactory.Create(_iconClass, new InlayController(), banner, IconResolution.High);
            _icon.Initialize();
            Insert(1, _icon);
        }
    } 
}
