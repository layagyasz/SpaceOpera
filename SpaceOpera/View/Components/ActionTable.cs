using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.View.Components.Dynamics;

namespace SpaceOpera.View.Components
{
    public class ActionTable<T> : DynamicUiCompoundComponent
    {
        public UiCompoundComponent Header { get; }
        public UiCompoundComponent Table { get; }

        public ActionTable(Class @class, UiCompoundComponent header, IUiContainer table)
            : base(
                  new ActionTableController<T>(),
                  new DynamicUiSerialContainer(
                      @class, new NoOpElementController(), UiSerialContainer.Orientation.Vertical))
        {
            Header = header;
            Table = new DynamicUiCompoundComponent(new RadioController<T>(), table);

            Add(Header);
            Add(Table);
        }
    }
}
