using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Core.Advancement;
using SpaceOpera.View.Components.Dynamics;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.Game.Panes.ResearchPanes
{
    public class AdvancementSlotComponent : DynamicUiCompoundComponent
    {
        public class Style
        {
            public string? Container { get; set; }
            public string? EmptyText { get; set; }
            public AdvancementComponent.Style? Advancement { get; set; }
        }

        public AdvancementSlot Slot { get; }
        public FactionAdvancementManager AdvancementManager { get; }

        private readonly Style _style;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private IAdvancement? _advancement;

        public AdvancementSlotComponent(
            AdvancementSlot slot,
            FactionAdvancementManager advancementManager, 
            Style style,
            UiElementFactory uiElementFactory, 
            IconFactory iconFactory)
            : base(
                  new NoOpController(), 
                  new DynamicUiSerialContainer(
                      uiElementFactory.GetClass(style.Container!),
                      new OptionElementController<AdvancementSlot>(slot),
                      UiSerialContainer.Orientation.Horizontal))
        {
            Slot = slot;
            AdvancementManager = advancementManager;
            _style = style;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;

            SetEmpty();
        }

        public override void Refresh()
        {
            if (Slot.Advancement != _advancement)
            {
                var newAdvancement = Slot.Advancement;
                if (newAdvancement == null)
                {
                    SetEmpty();
                }
                else
                {
                    SetAdvancement(newAdvancement);
                }
                _advancement = newAdvancement;
            }
            base.Refresh();
        }

        private void SetEmpty()
        {
            Clear(dispose: true);
            Add(
                new TextUiElement(
                    _uiElementFactory.GetClass(_style.EmptyText!), new InlayController(), "Select Research"));
        }

        private void SetAdvancement(IAdvancement advancement)
        {
            Clear(dispose: true);
            Add(
                AdvancementComponent.Create(
                    advancement, AdvancementManager, _style.Advancement!, _uiElementFactory, _iconFactory));
        }
    }
}
