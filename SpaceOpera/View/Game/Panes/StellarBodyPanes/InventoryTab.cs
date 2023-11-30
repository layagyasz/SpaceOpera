using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using Cardamom.Ui;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Game.Panes.Common;
using SpaceOpera.View.Icons;
using SpaceOpera.Core.Economics;
using SpaceOpera.Controller.Game.Panes;

namespace SpaceOpera.View.Game.Panes.StellarBodyPanes
{
    public class InventoryTab : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "stellar-body-pane-body";
        private static readonly InventoryComponent.Style s_Style = new()
        {
            Container = "stellar-body-pane-inventory-table",
            RowContainer = "stellar-body-pane-inventory-table-row",
            Info = "stellar-body-pane-inventory-table-row-info",
            Icon = "stellar-body-pane-inventory-table-row-icon",
            Text = "stellar-body-pane-inventory-table-row-text",
            Quantity = "stellar-body-pane-inventory-table-row-quantity"
        };

        public InventoryComponent Inventory { get; }

        public InventoryTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new NoOpTabController(),
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new NoOpElementController(),
                      UiSerialContainer.Orientation.Horizontal))
        {
            Inventory = InventoryComponent.Create(s_Style, uiElementFactory, iconFactory);
            Add(Inventory);
        }

        public void SetInventory(Inventory? inventory)
        {
            Inventory.SetInventory(inventory);
        }
    }
}
