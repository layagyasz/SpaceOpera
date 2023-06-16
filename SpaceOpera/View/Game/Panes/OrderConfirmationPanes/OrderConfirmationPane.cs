using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Game.Panes.OrderConfirmationPanes;
using SpaceOpera.Core.Orders;
using SpaceOpera.View.Game.Info;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.OrderConfirmationPanes
{
    public class OrderConfirmationPane : SimpleGamePane
    {
        private static readonly string s_Container = "order-confirmation-pane";
        private static readonly string s_Title = "order-confirmation-pane-title";
        private static readonly string s_Close = "order-confirmation-pane-close";
        private static readonly string s_BodyClass = "order-confirmation-pane-body";
        private static readonly string s_ConfirmClass = "order-confirmation-pane-confirm";
        private static readonly InfoPanel.Style s_InfoStyle =
            new()
            {
                Container = "order-confirmation-pane-info-container",
                Row = "order-confirmation-pane-info-row",
                RowHeading = "order-confirmation-pane-info-heading",
                RowValue = "order-confirmation-pane-info-value",
                MaterialCell = "order-confirmation-pane-info-material-cell",
                MaterialIcon = "order-confirmation-pane-info-material-icon",
                MaterialText = "order-confirmation-pane-info-material-text"
            };

        public IUiElement Confirm { get; }

        private IOrder? _order;

        private readonly InfoPanel _info;

        public OrderConfirmationPane(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new OrderConfirmationPaneController(), 
                  uiElementFactory.GetClass(s_Container),
                  new TextUiElement(uiElementFactory.GetClass(s_Title), new ButtonController(), string.Empty),
                  uiElementFactory.CreateSimpleButton(s_Close).Item1)
        {
            Confirm = uiElementFactory.CreateTextButton(s_ConfirmClass, "Confirm").Item1;
            _info = new(s_InfoStyle, uiElementFactory, iconFactory);
            SetBody(
                new UiSerialContainer(
                    uiElementFactory.GetClass(s_BodyClass),
                    new NoOpElementController<UiSerialContainer>(),
                    UiSerialContainer.Orientation.Vertical) 
                { 
                    _info,
                    Confirm
                });
        }

        public IOrder GetOrder()
        {
            return _order!;
        }

        public override void Populate(params object?[] args)
        {
            _order = args[0] as IOrder;
            _info.Clear(/* dispose= */ true);
            new OrderDescriber().Describe(_order!, _info);
            SetTitle("Confirm " + ToString(_order!));
            Populated?.Invoke(this, EventArgs.Empty);
        }

        private static string ToString(IOrder order)
        {
            if (order is BuildOrder)
            {
                return "Build Order";
            }
            throw new ArgumentException($"Unsupported order type: {order.GetType()}");
        }
    }
}
