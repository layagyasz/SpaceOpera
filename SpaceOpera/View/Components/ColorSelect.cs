using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.Components;

namespace SpaceOpera.View.Components
{
    public class ColorSelect : UiRootComponent
    {
        public class Style
        {
            public string? Root { get; set; }
            public string? OptionContainer { get; set; }
            public string? OptionRow { get; set; }
            public string? Option { get; set; }
            public int OptionsRowSize { get; set; } = 1;
        }

        public ColorSwatch Root { get; }
        public IUiComponent Options { get; }

        private ColorSelect(ColorSwatch root, IUiComponent options) 
            : base(new ColorSelectController(), root)
        {
            Root = root;
            Options = options;
            Options.Position = root.LeftMargin + new Vector3(root.TrueSize.X, 0, 0);
            Options.Visible = false;

            Add(options);
        }

        public void SetOpen(bool open)
        {
            Options.Visible = open;
        }

        public static ColorSelect Create(UiElementFactory uiElementFactory, Style style, IEnumerable<Color4> options)
        {
            var table = 
                new UiSerialContainer(
                    uiElementFactory.GetClass(style.OptionContainer!), 
                    new ButtonController(), 
                    UiSerialContainer.Orientation.Vertical);
            foreach (var rowOptions in options.Chunk(style.OptionsRowSize))
            {
                var row =
                    new UiSerialContainer(
                        uiElementFactory.GetClass(style.OptionRow!),
                        new InlayController(),
                        UiSerialContainer.Orientation.Horizontal);
                foreach (var option in  rowOptions)
                {
                    row.Add(
                        new ColorSwatch(
                            uiElementFactory.GetClass(style.Option!),
                            new OptionElementController<Color4>(option), 
                            option));
                }
                table.Add(row);
            }
            return new ColorSelect(
                new ColorSwatch(uiElementFactory.GetClass(style.Root!), new RootElementController(), options.First()),
                new UiCompoundComponent(new RadioController<Color4>(options.First(), bindRecursionDepth: 1), table));
        }
    }
}
