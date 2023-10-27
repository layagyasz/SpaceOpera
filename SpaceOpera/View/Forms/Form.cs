using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Forms
{
    public class Form : UiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? Paragraph { get; set; }
            public string? FieldHeader { get; set; }
            public Select.Style? DropDown { get; set; }
            public DialSelect.Style? Dial { get; set; }
            public Radio.Style? Radio { get; set; }
        }

        public string Title { get; }
        public bool AutoSubmit { get; }
        public Dictionary<string, IUiComponent> Fields { get; }

        public Form(
            IController controller,
            UiSerialContainer container,
            string title,
            Dictionary<string, IUiComponent> components, 
            bool autoSubmit)
            : base(controller, container)
        {
            Title = title;
            Fields = components;
            AutoSubmit = autoSubmit;
        }
    }
}
