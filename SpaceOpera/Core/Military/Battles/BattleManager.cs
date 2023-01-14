using Cardamom.Collections;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleManager
    {
        class BattleKey
        {
            public INavigable Position { get; }

            private BattleKey(INavigable position)
            {
                Position = position;
            }

            public static BattleKey Create(INavigable position)
            {
                return new BattleKey(position);
            }

            public override bool Equals(object? @object)
            {
                if (@object is BattleKey other)
                {
                    return Equals(other?.Position, Position);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Position.GetHashCode();
            }
        }

        public DiplomaticRelationGraph DiplomaticRelationGraph { get; }

        private MultiMap<BattleKey, Battle> _activeBattles = new();

        public BattleManager(DiplomaticRelationGraph diplomaticRelationGraph)
        {
            DiplomaticRelationGraph = diplomaticRelationGraph;
        }

        public bool CanEngage(IFormation formation, IFormation target)
        {
            if (!DiplomaticRelationGraph.CanAttack(formation.Faction, target.Faction))
            {
                return false;
            }
            var currentBattle = GetCurrentBattle(target, BattleKey.Create(target.Position!));
            if (currentBattle == null)
            {
                return true;
            }
            return CanParticipate(formation, currentBattle.Battle, ReverseSide(currentBattle.Side));
        }

        public bool CanParticipate(IFormation formation, Battle battle, BattleSideType side)
        {
            if (!battle.GetFormations(side)
                .Select(x => x.Faction)
                .Distinct()
                .All(x => x == formation.Faction || !DiplomaticRelationGraph.CanAttack(formation.Faction, x)))
            {
                return false;
            }
            if (!battle.GetFormations(ReverseSide(side))
                .Select(x => x.Faction)
                .Distinct()
                .All(x => x != formation.Faction && DiplomaticRelationGraph.CanAttack(formation.Faction, x)))
            {
                return false;
            }
            return true;
        }
        
        public void Engage(IFormation formation, IFormation target)
        {
            formation.EnterCombat();
            var key = BattleKey.Create(target.Position!);
            var currentBattle = GetCurrentBattle(target, key);
            if (currentBattle == null)
            {
                target.EnterCombat();
                var newBattle = new Battle();
                newBattle.Add(formation, BattleSideType.Offense);
                newBattle.Add(target, BattleSideType.Defense);
                _activeBattles.Add(key, newBattle);
            }
            else
            {
                currentBattle.Battle.Add(formation, ReverseSide(currentBattle.Side));
            }
        }

        public void Tick(Random random)
        {
            var newActiveBattles = new MultiMap<BattleKey, Battle>();
            foreach (var battlesAndKey in _activeBattles)
            {
                foreach (var battle in battlesAndKey.Value.ToList())
                {
                    battle.Tick(random);
                    foreach (var formation in battle.GetFormations(BattleSideType.Offense).ToList())
                    {
                        if (formation.Cohesion.IsEmpty())
                        {
                            Disengage(formation, battle, BattleSideType.Offense);
                        }
                    }
                    foreach (var formation in battle.GetFormations(BattleSideType.Defense).ToList())
                    {
                        if (formation.Cohesion.IsEmpty())
                        {
                            Disengage(formation, battle, BattleSideType.Defense);
                        }
                    }
                    if (!battle.GetFormations(BattleSideType.Offense).Any()
                        || !battle.GetFormations(BattleSideType.Defense).Any())
                    {
                        foreach (var formation in battle.GetFormations(BattleSideType.Offense))
                        {
                            formation.ExitCombat();
                        }
                        foreach (var formation in battle.GetFormations(BattleSideType.Defense))
                        {
                            formation.ExitCombat();
                        }
                    }
                    else
                    {
                        newActiveBattles.Add(battlesAndKey);
                    }
                    battle.GetReport().Print();
                    Console.ReadLine();
                }
            }
            _activeBattles = newActiveBattles;
        }

        private static void Disengage(IFormation formation, Battle battle, BattleSideType side)
        {
            formation.ExitCombat();
            battle.Remove(formation, side);
        }

        private BattleParticipation? GetCurrentBattle(IFormation formation, BattleKey key)
        {
            if (!formation.InCombat)
            {
                return null;
            }
            foreach (var battle in _activeBattles[key])
            {
                var side = battle.GetBattleSide(formation);
                switch (side)
                {
                    case BattleSideType.None:
                        continue;
                    case BattleSideType.Offense:
                    case BattleSideType.Defense:
                        return new BattleParticipation(battle, side);
                    default:
                        throw new InvalidProgramException();
                }
            }
            return null;
        }

        private static BattleSideType ReverseSide(BattleSideType Side)
        {
            return Side switch
            {
                BattleSideType.Offense => BattleSideType.Defense,
                BattleSideType.Defense => BattleSideType.Offense,
                _ => BattleSideType.None,
            };
        }
    }
}