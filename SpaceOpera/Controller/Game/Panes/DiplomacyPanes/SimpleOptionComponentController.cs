using Cardamom.Ui;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class SimpleOptionComponentController : IDiplomacyOptionController
    {
        public EventHandler<IDiplomaticAgreementSection>? Added { get; set; }
        public EventHandler<PopupEventArgs>? PopupCreated { get; set; }

        private IUiComponent? _component;
        private readonly Func<IDiplomaticAgreementSection> _producerFn;

        public SimpleOptionComponentController(Func<IDiplomaticAgreementSection> producerFn)
        {
            _producerFn = producerFn;
        }

        public void Bind(object @object)
        {
            _component = (IUiComponent)@object!;
            _component.Controller.Clicked += HandleClick;
        }

        public void Unbind()
        {
            _component!.Controller.Clicked -= HandleClick;
            _component = null;
        }

        private void HandleClick(object? sender, MouseButtonClickEventArgs e)
        {
            Added?.Invoke(this, _producerFn());
        }
    }
}
