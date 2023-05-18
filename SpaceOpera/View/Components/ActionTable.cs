﻿using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;

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
                      @class, new NoOpElementController<UiSerialContainer>(), UiSerialContainer.Orientation.Vertical))
        {
            Header = header;
            Table = new DynamicUiCompoundComponent(new RadioController<T>($"action-table-{GetHashCode()}"), table);

            Add(Header);
            Add(Table);
        }
    }
}