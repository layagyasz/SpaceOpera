using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core.Economics;
using SpaceOpera.View.Components;
using SpaceOpera.View.Forms;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.StellarBodyPanes
{
    public class OverviewTab : DynamicUiCompoundComponent
    {
        private static readonly string s_Container = "stellar-body-pane-body";
        private static readonly string s_Column = "stellar-body-pane-body-column";

        private Form.Style s_Style =
            new()
            {
                Container = s_Container,
                IconTitle = "stellar-body-pane-overview-icon-title",
                Header1 = "stellar-body-pane-overview-h1",
                Header3 = "stellar-body-pane-overview-h3",
                Paragraph = "stellar-body-pane-overview-p"
            };

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        public OverviewTab(UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  new NoOpController(), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(s_Container),
                      new TableController(10f),
                      Cardamom.Ui.Elements.UiSerialContainer.Orientation.Vertical))
        {
            _uiElementFactory = uiElementFactory; 
            _iconFactory = iconFactory;
        }

        public void SetHolding(StellarBodyHolding holding)
        {
            Clear(/* dispose= */ true);
            var layout =
                new FormLayout.Builder()
                    .AddIcon()
                        .SetObject(holding.StellarBody)
                        .Complete();
            layout
                .AddHeader1()
                    .SetText("Surface")
                    .Complete()
                .AddText()
                    .SetName("Classification")
                    .SetText(holding.StellarBody.Type);
            var orbit = holding.StellarBody.Orbit;
            layout
                .AddHeader1()
                    .SetText("Orbit")
                    .Complete()
                .AddText()
                    .SetName("Solar Distance")
                    .SetText($"{orbit.GetAverageDistance():N2} au")
                    .Complete()
                .AddText()
                    .SetName("Eccentricity")
                    .SetText($"{orbit.Eccentricity:N2}")
                    .Complete()
                .AddText()
                    .SetName("Year Length")
                    .SetText($"{orbit.GetYearLengthInSeconds() * Constants.DaysPerSecond:N2} days")
                    .Complete()
                .AddText()
                    .SetName("Day Length")
                    .SetText($"{orbit.GetDayLengthInSeconds() * Constants.HoursPerSecond:N2} hours")
                    .Complete();
            var form = layout.Build().CreateForm(s_Style, _uiElementFactory, _iconFactory);
            form.Initialize();
            Add(form);
        }
    }
}
