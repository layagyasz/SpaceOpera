using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Loader;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Loader
{
    public class LoaderComponent : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "loader-progress-container";
        private static readonly string s_Text = "loader-progress-text";
        private static readonly string s_Progress = "loader-progress";

        public LoaderStatus Status { get; }

        public LoaderComponent(UiElementFactory uiElementFactory, LoaderStatus loaderStatus)
            : base(
                  new NoOpController<UiCompoundComponent>(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        {
            Status = loaderStatus;
            Add(new DynamicTextUiElement(uiElementFactory.GetClass(s_Text), new InlayController(), GetStatusText));
            Add(new PoolBar(uiElementFactory.GetClass(s_Progress), new InlayController(), loaderStatus.Progress));
            Add(new DynamicTextUiElement(uiElementFactory.GetClass(s_Text), new InlayController(), GetProgressText));
        }

        private string GetStatusText()
        {
            return Status.GetStatus().FirstOrDefault(string.Empty);
        }

        private string GetProgressText()
        {
            return string.Format("{0:P0}", Status.Progress.PercentFull());
        }
    }
}
