using Cardamom;

namespace SpaceOpera.Core.Politics
{
    public class DiplomaticRelationGraph
    {
        private readonly Dictionary<CompositeKey<Faction, Faction>, DiplomaticRelation> _relations = new();

        public void AddFaction(Faction newFaction, IEnumerable<Faction> factions)
        {
            foreach (var target in factions)
            {
                if (newFaction != target)
                {
                    _relations.Add(
                        CompositeKey<Faction, Faction>.Create(newFaction, target),
                        new DiplomaticRelation(newFaction, target));
                }
            }
        }

        public bool CanAttack(Faction faction, Faction target)
        {
            return Get(faction, target).Status == DiplomaticRelation.DiplomaticStatus.War;
        }

        public void Initialize(IEnumerable<Faction> factions)
        {
            foreach (var faction in factions)
            {
                AddFaction(faction, factions);
            }
        }

        public DiplomaticRelation Get(Faction faction, Faction target)
        {
            return _relations[CompositeKey<Faction, Faction>.Create(faction, target)];
        }
    }
}