using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.Common
{
    public class ProjectsComponent : DynamicUiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public ActionRowStyles.Style? RowContainer { get; set; }
            public string? Icon { get; set; }
            public string? Info { get; set; }
            public string? Text { get; set; }
            public string? Status { get; set; }
            public string? StatusText { get; set; }
            public string? StatusProgress { get; set; }
            public string? Cancel { get; set; }
        }

        class ProjectComponentFactory : IKeyedElementFactory<IProject>
        {
            private readonly Style _style;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public ProjectComponentFactory(Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
            {
                _style = style;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<IProject> Create(IProject project)
            {
                return ActionRow<IProject>.Create(
                    project,
                    ActionId.Unknown,
                    ActionId.Unknown,
                    _uiElementFactory,
                    _style.RowContainer!,
                    new List<IUiElement>()
                    {
                        _iconFactory.Create(
                            _uiElementFactory.GetClass(_style.Icon!), new InlayController(), project.Key),
                        new DynamicUiSerialContainer(
                            _uiElementFactory.GetClass(_style.Info!),
                            new NoOpElementController(),
                            UiSerialContainer.Orientation.Vertical)
                        {
                            new TextUiElement(
                                _uiElementFactory.GetClass(_style.Text!), new InlayController(), project.Name),
                            new DynamicUiSerialContainer(
                                _uiElementFactory.GetClass(_style.Status!),
                                new NoOpElementController(),
                                UiSerialContainer.Orientation.Vertical)
                            {
                                new DynamicTextUiElement(
                                    _uiElementFactory.GetClass(_style.StatusText!),
                                    new InlayController(),
                                    () => GetStatusString(project)),
                                new PoolBar(
                                    _uiElementFactory.GetClass(_style.StatusProgress!),
                                    new InlayController(),
                                    project.Progress),
                            }
                        }
                    },
                    new List<ActionRowStyles.ActionConfiguration>()
                    {
                        new ()
                        {
                            Button = _style.Cancel!,
                            Action = ActionId.Cancel
                        }
                    });
            }

            private static string GetStatusString(IProject project)
            {
                return $"{project.Progress.ToString("N0")} - {EnumMapper.ToString(project.Status)}";
            }
        }

        private readonly DelegatedRange<IProject> _range;

        private ProjectsComponent(
            Class @class, DelegatedRange<IProject> range, ProjectComponentFactory componentFactory)
            : base(
                    new ActionComponentController(),
                    DynamicKeyedContainer<IProject>.CreateSerial(
                        @class,
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        range.GetRange,
                        componentFactory,
                        Comparer<IProject>.Create(
                            (x, y) => y.Progress.PercentFull().CompareTo(x.Progress.PercentFull()))))
        {
            _range = range;
        }

        public void SetRange(KeyRange<IProject>? range)
        {
            _range.SetRange(range);
        }

        public static ProjectsComponent Create(Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var range = new DelegatedRange<IProject>();
            return new(
                uiElementFactory.GetClass(style.Container!),
                range,
                new ProjectComponentFactory(style, uiElementFactory, iconFactory));
        }
    }
}
