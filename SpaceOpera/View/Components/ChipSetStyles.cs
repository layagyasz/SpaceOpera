namespace SpaceOpera.View.Components
{
    public class ChipSetStyles
    {
        public class ChipSetStyle
        {
            public string? Container { get; set; }
            public ChipStyle? Chip { get; set; }
        }

        public class ChipStyle 
        {
            public string? Container { get; set; }
            public string? Icon { get; set; }
            public string? Text { get; set; }
        }
    }
}
