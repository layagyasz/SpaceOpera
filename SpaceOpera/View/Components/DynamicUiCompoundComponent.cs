﻿using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Elements;

namespace SpaceOpera.View.Components
{
    public class DynamicUiCompoundComponent : UiCompoundComponent, IDynamic
    {
        public EventHandler<EventArgs>? Refreshed { get; set; }

        public DynamicUiCompoundComponent(IController componentController, IUiContainer container)
            : base(componentController, container) { }

        public void Refresh()
        {
            if (_container is IDynamic dynamic)
            {
                dynamic.Refresh();
            }
            Refreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
