using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Politics
{
    class DiplomaticRelationGraph
    {
        private readonly Dictionary<CompositeKey<Faction, Faction>, DiplomaticRelation> _Relations =
            new Dictionary<CompositeKey<Faction, Faction>, DiplomaticRelation>();

        public void AddFaction(Faction NewFaction, IEnumerable<Faction> Factions)
        {
            foreach (var target in Factions)
            {
                if (NewFaction != target)
                {
                    _Relations.Add(
                        CompositeKey<Faction, Faction>.Create(NewFaction, target),
                        new DiplomaticRelation(NewFaction, target));
                }
            }
        }

        public bool CanAttack(Faction Faction, Faction Target)
        {
            return Get(Faction, Target).Status == DiplomaticRelation.DiplomaticStatus.WAR;
        }

        public void Initialize(IEnumerable<Faction> Factions)
        {
            foreach (var faction in Factions)
            {
                AddFaction(faction, Factions);
            }
        }

        public DiplomaticRelation Get(Faction Faction, Faction Target)
        {
            return _Relations[CompositeKey<Faction, Faction>.Create(Faction, Target)];
        }
    }
}