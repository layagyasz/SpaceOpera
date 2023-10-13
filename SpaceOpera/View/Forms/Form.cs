using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Forms
{
    public class Form : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? FieldHeader { get; set; }
        }

        public Dictionary<string, IUiComponent> Fields { get; }

        public Form(IController controller, UiSerialContainer container, Dictionary<string, IUiComponent> components)
            : base(controller, container)
        {
            Fields = components;
        }
    }
}
