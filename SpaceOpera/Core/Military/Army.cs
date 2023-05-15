﻿using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public class Army : IFormation
    {
        public string Name { get; private set; } = string.Empty;
        public Faction Faction { get; }
        public List<Division> Divisions { get; } = new();

        public Army(Faction faction)
        {
            Faction = faction;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}