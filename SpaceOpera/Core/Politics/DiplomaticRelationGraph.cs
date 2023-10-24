using Cardamom;
using SpaceOpera.Core.Politics.Diplomacy;

namespace SpaceOpera.Core.Politics
{
    public class DiplomaticRelationGraph
    {
        private readonly List<Faction> _factions = new();
        private readonly Dictionary<CompositeKey<Faction, Faction>, DiplomaticRelation> _relations = new();

        public void Add(Faction newFaction)
        {
            foreach (var target in _factions)
            {
                _relations.Add(
                    CompositeKey<Faction, Faction>.Create(newFaction, target),
                    new DiplomaticRelation(newFaction, target));
                _relations.Add(
                    CompositeKey<Faction, Faction>.Create(target, newFaction),
                    new DiplomaticRelation(target, newFaction));
            }
            _factions.Add(newFaction);
        }

        public void Apply(World world, DiplomaticAgreement agreement)
        {
            var left = Get(agreement.Proposer, agreement.Approver);
            var right = Get(agreement.Approver, agreement.Proposer);

            // Cancel conflicting agreements
            foreach (var current in left.CurrentAgreements)
            {
                if (agreement.Cancels(current))
                {
                    current.Cancel(world);
                    left.Cancel(current);
                }
            }
            foreach (var current in right.CurrentAgreements)
            {
                if (agreement.Cancels(current))
                {
                    current.Cancel(world);
                    right.Cancel(current);
                }
            }

            // Apply the new agreement
            left.Add(agreement);
            right.Add(agreement);
            agreement.Apply(world, left, right);

            // Notify any existing agreements of the change
            foreach (var relation in world.DiplomaticRelations.GetAsTarget(agreement.Proposer).Where(x => x != right))
            {
                foreach (var current in relation.CurrentAgreements)
                {
                    current.Notify(world, relation, agreement, /* isProposer= */ true);
                }
            }
            foreach (var relation in world.DiplomaticRelations.GetAsTarget(agreement.Approver).Where(x => x != left))
            {
                foreach (var current in relation.CurrentAgreements)
                {
                    current.Notify(world, relation, agreement, /* isProposer= */ false);
                }
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