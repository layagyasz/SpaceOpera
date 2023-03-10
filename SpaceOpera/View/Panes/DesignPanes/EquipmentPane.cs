using Cardamom.Ui;
using SpaceOpera.Core.Designs;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Panes.DesignPanes
{
    public class EquipmentPane : DesignPane
    {
        public EquipmentPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  uiElementFactory,
                  iconFactory, 
                  new ComponentType[] 
                  {
                      ComponentType.Ship,
                      ComponentType.ShipWeapon,
                      ComponentType.ShipShield
                  }) { }
    }
}
