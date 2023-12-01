using SpaceOpera.Core.Events;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Orders.Economics;
using SpaceOpera.View.Forms;

namespace SpaceOpera.View.Game.Panes.Forms
{
    public class FormHelper
    {
        public static FormLayout ForEvent(IEvent @event)
        {
            var field =
                new FormLayout.Builder()
                    .SetName(@event.GetTitle())
                    .AutoSubmit()
                    .AddHidden("event", @event)
                    .AddParagraph()
                        .SetText(@event.GetDescription())
                        .Complete()
                    .AddRadio()
                        .SetId("decisionId");
            foreach (var decision in @event.GetDecisions())
            {
                field.AddOption(decision.Description, decision.Id);
            }
            return (FormLayout)field.Complete().Build();
        }

        public static FormLayout ForOrder(IOrder order)
        {
            return
                (FormLayout)new FormLayout.Builder()
                    .SetName(ToConfirmationString(order))
                    .AutoSubmit()
                    .AddHidden("order", order)
                    .AddInfo(order)
                    .AddRadio()
                        .SetId("decisionId")
                        .AddOption("Confirm", 0)
                        .AddOption("Cancel", 1)
                    .Complete()
                    .Build();
        }

        private static string ToConfirmationString(IOrder order)
        {
            if (order is BuildOrder)
            {
                return "Confirm Build Order";
            }
            throw new ArgumentException($"Unsupported order type: {order.GetType()}");
        }
    }
}
