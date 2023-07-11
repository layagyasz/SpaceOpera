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
        public LoaderComponent(UiElementFactory uiElementFactory, LoaderStatus loaderStatus)
            : base(
                  new NoOpController<UiCompoundComponent>(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass("ui-container"),
                      new NoOpElementController<UiSerialContainer>(),
                      UiSerialContainer.Orientation.Vertical))
        { }
    }
}
