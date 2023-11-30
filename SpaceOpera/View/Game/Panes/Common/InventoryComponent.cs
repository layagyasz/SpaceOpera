using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.Common
{
    public class InventoryComponent : DynamicUiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? RowContainer { get; set; }
            public string? Info { get; set; }
            public string? Icon { get; set; }
            public string? Text { get; set; }
            public string? Quantity { get; set; }
        }

        record class InventoryKey(Inventory Inventory, IMaterial Material);

        class InventoryRange
        {
            private Inventory? _inventory;

            public IEnumerable<InventoryKey> GetRange()
            {
                foreach (var material in _inventory?.Contents.Keys ?? Enumerable.Empty<IMaterial>())
                {
                    yield return new(_inventory!, material);
                }
            }

            public void SetInventory(Inventory? inventory)
            {
                _inventory = inventory;
            }
        }

        class MaterialComponentFactory : IKeyedElementFactory<InventoryKey>
        {
            private readonly Style _style;
            private readonly UiElementFactory _uiElementFactory;
            private readonly IconFactory _iconFactory;

            public MaterialComponentFactory(Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
            {
                _style = style;
                _uiElementFactory = uiElementFactory;
                _iconFactory = iconFactory;
            }

            public IKeyedUiElement<InventoryKey> Create(InventoryKey key)
            {
                var material = key.Material;
                return KeyedUiElementWrapper<InventoryKey>.Wrap(
                    key, 
                    new DynamicUiSerialContainer(
                        _uiElementFactory.GetClass(_style.RowContainer!),
                        new ButtonController(),
                        UiSerialContainer.Orientation.Horizontal)
                        {
                            new DynamicUiSerialContainer(
                                _uiElementFactory.GetClass(_style.Info!),
                                new NoOpElementController(),
                                UiSerialContainer.Orientation.Horizontal)
                            {
                                _iconFactory.Create(
                                    _uiElementFactory.GetClass(_style.Icon!), new InlayController(), material),
                                new TextUiElement(
                                    _uiElementFactory.GetClass(_style.Text!), new InlayController(), material.Name)
                            },
                            new DynamicTextUiElement(
                                _uiElementFactory.GetClass(_style.Quantity!), 
                                new InlayController(), 
                                () => key.Inventory.Contents[material].ToString("N0"))
                        });
            }
        }

        private readonly InventoryRange _range;

        private InventoryComponent(Class @class, InventoryRange range, MaterialComponentFactory componentFactory)
            : base(
                  new NoOpController(),
                  DynamicKeyedContainer<InventoryKey>.CreateSerial(
                        @class,
                        new TableController(10f),
                        UiSerialContainer.Orientation.Vertical,
                        range.GetRange,
                        componentFactory,
                        Comparer<InventoryKey>.Create((x, y) => y.Material.Name.CompareTo(x.Material.Name))))
        {
            _range = range;
        }

        public void SetInventory(Inventory? inventory)
        {
            _range.SetInventory(inventory);
        }

        public static InventoryComponent Create(
            Style style, UiElementFactory uiElementFactory, IconFactory iconFactory)
        {
            var range = new InventoryRange();
            return new(
                uiElementFactory.GetClass(style.Container!),
                range,
                new MaterialComponentFactory(style, uiElementFactory, iconFactory));
        }
    }
}
