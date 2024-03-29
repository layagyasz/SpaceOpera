﻿using Cardamom;

namespace SpaceOpera.Core.Designs
{
    public class SegmentConfiguration : IKeyed
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public BaseComponent? IntrinsicComponent { get; set; }
        public DesignSlot[] Slots { get; set; } = Array.Empty<DesignSlot>();
    }
}
