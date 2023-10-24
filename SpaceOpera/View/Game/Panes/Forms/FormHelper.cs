using SpaceOpera.Core.Events;
using SpaceOpera.View.Forms;

namespace SpaceOpera.View.Game.Panes.Forms
{
    public class FormHelper
    {
        public static FormLayout ForEvent(IEvent @event)
        {
            var field =
                new FormLayout.Builder()
                    .SetTitle(@event.Title)
                    .AutoSubmit()
                    .AddHidden("event", @event)
                    .AddRadio()
                        .SetId("decisionId");
            foreach (var decision in @event.GetDecisions())
            {
                field.AddOption(decision.Description, decision.Id);
            }
            return field.CompleteField().Build();
        }
    }
}
