﻿using Cardamom.Ui;

namespace SpaceOpera.View.Game.Panes
{
    public interface IGamePane : IUiContainer, IDynamic
    {
        EventHandler<EventArgs>? Populated { get; set; }

        void Populate(params object?[] args);
    }
}
