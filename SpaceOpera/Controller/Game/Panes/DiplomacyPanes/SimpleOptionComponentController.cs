using SpaceOpera.Controller.Game.Panes.DiplomacyPanes;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Controller.Components
{
    public class SimpleOptionComponentController 
        : FuncAdderController<IDiplomaticAgreementSection>, IDiplomacyOptionController
    {
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        public SimpleOptionComponentController(Func<IDiplomaticAgreementSection> producerFn)
            : base(producerFn) { }
    }
}
