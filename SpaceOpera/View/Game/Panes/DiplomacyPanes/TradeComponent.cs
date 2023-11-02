using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.Components;
using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.View.Game.Panes.DiplomacyPanes
{
    public class TradeComponent : UiCompoundComponent
    {
        private static readonly string s_Container = "diplomacy-pane-diplomacy-side-trade-section";
        private static readonly string s_Header = "diplomacy-pane-diplomacy-side-trade-section-header";
        private static readonly string s_OptionTable = "diplomacy-pane-diplomacy-side-trade-section-option-table";
        private static readonly string s_Option = "diplomacy-pane-diplomacy-side-trade-section-option";

        public IUiElement Header { get; }
        public IUiComponent Options { get; }

        private TradeComponent(Class @class, IController controller, IUiElement header, IUiComponent options)
            : base(
                  controller, 
                  new UiSerialContainer(@class, new TableController(0f), UiSerialContainer.Orientation.Vertical))
        {
            Header = header;
            Options = options;

            Add(Header);
            Add(Options);
        }

        public static TradeComponent Create(UiElementFactory uiElementFactory, World world, Faction faction)
        {
            var optionsTable =
                new UiCompoundComponent(
                    new AdderComponentController<StellarBodyHolding>(),
                    new UiSerialContainer(
                        uiElementFactory.GetClass(s_OptionTable),
                        new TableController(0f), 
                        UiSerialContainer.Orientation.Vertical));
            var optionClass = uiElementFactory.GetClass(s_Option);
            foreach (var option in world.Economy.GetHoldingsFor(faction))
            {
                optionsTable.Add(
                    new UiSimpleComponent(
                        new StaticAdderController<StellarBodyHolding>(option),
                        new TextUiElement(optionClass, new ButtonController(), option.Name)));
            }
            return new(
                uiElementFactory.GetClass(s_Container),
                new TradeComponentController(world, faction),
                new TextUiElement(
                    uiElementFactory.GetClass(s_Header), new ButtonController(), DiplomacyType.Trade.Name),
                optionsTable);
        }
    }
}
