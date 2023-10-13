﻿using Cardamom.Ui;
using SpaceOpera.Controller.Components;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Controller.Game.Panes.DiplomacyPanes
{
    public class SimpleSectionComponentController : IAdderController<IDiplomaticAgreementSection>
    {
        public EventHandler<IDiplomaticAgreementSection>? Added { get; set; }

        private IUiComponent? _component;
        private readonly IDiplomaticAgreementSection _section;

        public SimpleSectionComponentController(IDiplomaticAgreementSection section)
        {
            _section = section;
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
            Added?.Invoke(this, _section);
        }
    }
}