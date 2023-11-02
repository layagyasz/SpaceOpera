using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components.NumericInputs
{
    public static class MultiCountInputStyles
    {
        public class MultiCountInputRowStyle
        {
            public string? Container { get; set; }
            public string? Info { get; set; }
            public string? Icon { get; set; }
            public string? Text { get; set; }
            public NumericInput.Style? NumericInput { get; set; }
        }

        public class ManualMultiCountInputRowStyle : MultiCountInputRowStyle
        {
            public string? Remove { get; set; }
        }

        public class MultiCountInputStyle
        {
            public string? Container { get; set; }
            public string? Table { get; set; }
            public MultiCountInputRowStyle? Row { get; set; }
            public string? TotalContainer { get; set; }
            public string? TotalText { get; set; }
            public string? TotalNumber { get; set; }
        }

        public class ManualMultiCountInputStyle : MultiCountInputStyle
        {
            public string? SelectWrapper { get; set; }
            public Select.Style? Select { get; set; }
            public string? Add { get; set; }
        }
    }
}
