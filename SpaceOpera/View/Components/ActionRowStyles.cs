namespace SpaceOpera.View.Components
{
    public static class ActionRowStyles
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? ActionContainer { get; set; }
        }

        public class ActionConfiguration
        {
            public string? Button { get; set; }
            public ActionId Action { get; set; }
        }
    }
}
