using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.GameSetup;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.GameSetup
{
    public class GalaxyComponent : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? SectionHeader { get; set; }
            public string? FieldHeader { get; set; }
            public DialSelect.Style? Select { get; set; }
        }

        private static readonly List<SelectOption<float>> s_RadiusOptions =
            new()
            {
                SelectOption<float>.Create(25000, "Tiny"),
                SelectOption<float>.Create(35000, "Small"),
                SelectOption<float>.Create(50000, "Average"),
                SelectOption<float>.Create(70000, "Large"),
                SelectOption<float>.Create(100000, "Huge"),
            };

        private static readonly List<SelectOption<int>> s_ShapeOptions =
            new()
            {
                SelectOption<int>.Create(0, "Disk"),
                SelectOption<int>.Create(2, "Barrel"),
                SelectOption<int>.Create(4, "Spiral"),
            };

        private static readonly List<SelectOption<float>> s_RotationOptions =
            new()
            {
                SelectOption<float>.Create(4, "Shallow"),
                SelectOption<float>.Create(8, "Standard"),
                SelectOption<float>.Create(16, "Steep"),
            };

        private static readonly List<SelectOption<float>> s_StarDensityOptions =
            new()
            {
                SelectOption<float>.Create(6.366e-9f, "Very Sparse"),
                SelectOption<float>.Create(1.2732e-8f, "Sparse"),
                SelectOption<float>.Create(2.5464e-8f, "Average"),
                SelectOption<float>.Create(5.0928e-8f, "Dense"),
                SelectOption<float>.Create(1.0186e-7f, "Very Dense"),
            };

        private static readonly List<SelectOption<float>> s_TransitDensityOptions =
            new()
            {
                SelectOption<float>.Create(0.0125f, "Very Sparse"),
                SelectOption<float>.Create(0.025f, "Sparse"),
                SelectOption<float>.Create(0.05f, "Average"),
                SelectOption<float>.Create(0.1f, "Dense"),
                SelectOption<float>.Create(0.2f, "Very Dense"),
            };

        public IUiComponent Radius { get; }
        public IUiComponent Shape { get; }
        public IUiComponent Rotation { get; }
        public IUiComponent StarDensity { get; }
        public IUiComponent TransitDensity { get; }

        public GalaxyComponent(UiElementFactory uiElementFactory, Style style)
            : base(
                  new GalaxyComponentController(), 
                  new UiSerialContainer(
                      uiElementFactory.GetClass(style.Container!), 
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Radius = DialSelect.Create(uiElementFactory, style.Select!, s_RadiusOptions, s_RadiusOptions[2].Value);
            Shape = DialSelect.Create(uiElementFactory, style.Select!, s_ShapeOptions, s_ShapeOptions[2].Value);
            Rotation = 
                DialSelect.Create(uiElementFactory, style.Select!, s_RotationOptions, s_RotationOptions[2].Value);
            StarDensity = 
                DialSelect.Create(
                    uiElementFactory, style.Select!, s_StarDensityOptions, s_StarDensityOptions[2].Value);
            TransitDensity = 
                DialSelect.Create(
                    uiElementFactory, style.Select!, s_TransitDensityOptions, s_TransitDensityOptions[2].Value);

            Add(new TextUiElement(uiElementFactory.GetClass(style.SectionHeader!), new ButtonController(), "Galaxy"));
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Radius"));
            Add(Radius);
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Shape"));
            Add(Shape);
            Add(new TextUiElement(uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Rotation"));
            Add(Rotation);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Stellar Density"));
            Add(StarDensity);
            Add(
                new TextUiElement(
                    uiElementFactory.GetClass(style.FieldHeader!), new ButtonController(), "Jump Density"));
            Add(TransitDensity);
        }
    }
}
