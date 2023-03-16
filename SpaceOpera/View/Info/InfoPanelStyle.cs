namespace SpaceOpera.View.Info
{
    public class InfoPanelStyle
    {
        public string? ContainerClass { get; }
        public string? HeaderRowClass { get; }
        public string? HeaderIconClass { get; }
        public string? HeaderTextClass { get; }
        public string? RowClass { get; }
        public string? RowHeadingClass { get; }
        public string? RowValueClass { get; }
        public string? MaterialCellClass { get; }
        public string? MaterialIconClass { get; }
        public string? MaterialTextClass { get; }

        public InfoPanelStyle(
            string? containerClass,
            string? headerRowClass,
            string? headerIconClass,
            string? headerTextClass,
            string? rowClass,
            string? rowHeadingClass,
            string? rowValueClass,
            string? materialCellClass,
            string? materialIconClass,
            string? materialTextClass)
        {
            ContainerClass = containerClass;
            HeaderRowClass = headerRowClass;
            HeaderIconClass = headerIconClass;
            HeaderTextClass = headerTextClass;
            RowClass = rowClass;
            RowHeadingClass = rowHeadingClass;
            RowValueClass = rowValueClass;
            MaterialCellClass = materialCellClass;
            MaterialIconClass = materialIconClass;
            MaterialTextClass = materialTextClass;
        }
    }
}
