﻿using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Components.NumericInputs;
using SpaceOpera.View.Info;

namespace SpaceOpera.View.Forms
{
    public class Form : DynamicUiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? IconTitle { get; set; }
            public string? Header1 { get; set; }
            public string? Paragraph { get; set; }
            public string? Header3 { get; set; }
            public InfoPanel.Style Info { get; set; }
            public ChipSetStyles.ChipSetStyle? ChipSet { get; set; }
            public Select.Style? DropDown { get; set; }
            public DialSelect.Style? Dial { get; set; }
            public Radio.Style? Radio { get; set; }
            public MultiCountInputStyles.ManualMultiCountInputStyle? MultiCount { get; set; }
        }

        public string Name { get; }
        public bool AutoSubmit { get; }
        public Dictionary<string, IUiComponent> Fields { get; }

        public Form(
            IController controller,
            UiSerialContainer container,
            string name,
            Dictionary<string, IUiComponent> components, 
            bool autoSubmit)
            : base(controller, container)
        {
            Name = name;
            Fields = components;
            AutoSubmit = autoSubmit;
        }
    }
}
