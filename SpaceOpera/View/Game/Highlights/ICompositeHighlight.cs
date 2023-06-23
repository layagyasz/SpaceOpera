﻿namespace SpaceOpera.View.Game.Highlights
{
    public interface ICompositeHighlight
    {
        EventHandler<ValueEventArgs<IHighlight>>? HighlightAdded { get; set; }
        IEnumerable<IHighlight> GetHighlights();
        void Unhook();
    }
}