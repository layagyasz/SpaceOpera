using Cardamom;

namespace SpaceOpera.Core.Politics
{
    public class DiplomaticRelationGraph
    {
        private readonly List<Faction> _factions = new();
        private readonly Dictionary<CompositeKey<Faction, Faction>, DiplomaticRelation> _relations = new();

        public void AddFaction(Faction newFaction)
        {
            foreach (var target in _factions)
            {
                _relations.Add(
                    CompositeKey<Faction, Faction>.Create(newFaction, target),
                    new DiplomaticRelation(newFaction, target));
                _relations.Add(
                    CompositeKey<Faction, Faction>.Create(target, newFaction),
                    new DiplomaticRelation(newFaction, target));
            }
            _factions.Add(newFaction);
        }

        public void AddAllFactions(IEnumerable<Faction> factions)
        {
            foreach (var faction in factions)
            {
                AddFaction(faction);
            }
        }

        public bool CanAttack(Faction faction, Faction target)
        {
            return Get(faction, target).OverallStatus == DiplomaticRelation.DiplomaticStatus.War;
        }

        public IEnumerable<DiplomaticRelation> Get(Faction faction)
        {
            return _relations.Where(x => x.Key.Key1 == faction).Select(x => x.Value);
        }

        public IEnumerable<DiplomaticRelation> GetAsTarget(Faction faction)
        {
            return _relations.Where(x => x.Key.Key2 == faction).Select(x => x.Value);
        }

        public DiplomaticRelation Get(Faction faction, Faction target)
        {
            return _relations[CompositeKey<Faction, Faction>.Create(faction, target)];
        }
    }
}