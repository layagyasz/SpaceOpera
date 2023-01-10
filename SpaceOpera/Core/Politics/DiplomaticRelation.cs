using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics
{
    class DiplomaticRelation
    {
        public enum DiplomaticStatus
        {
            NONE,

            PEACE,
            WAR
        }

        public Faction Faction { get; }
        public Faction Target { get; }
        public DiplomaticStatus Status { get; } = DiplomaticStatus.WAR;

        public DiplomaticRelation(Faction Faction, Faction Target)
        {
            this.Faction = Faction;
            this.Target = Target;
        }
    }
}