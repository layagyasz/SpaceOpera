using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Battles
{
    class BattleManager
    {
        class BattleKey
        {
            public INavigable Position { get; }

            private BattleKey(INavigable Position)
            {
                this.Position = Position;
            }

            public static BattleKey Create(INavigable Position)
            {
                return new BattleKey(Position);
            }

            public override bool Equals(object Object)
            {
                if (Object is BattleKey other)
                {
                    return other.Position == Position;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Position.GetHashCode();
            }
        }

        public DiplomaticRelationGraph DiplomaticRelationGraph { get; }

        private MultiMap<BattleKey, Battle> _ActiveBattles = new MultiMap<BattleKey, Battle>();

        public BattleManager(DiplomaticRelationGraph DiplomaticRelationGraph)
        {
            this.DiplomaticRelationGraph = DiplomaticRelationGraph;
        }

        public bool CanEngage(IFormation Formation, IFormation Target)
        {
            if (!DiplomaticRelationGraph.CanAttack(Formation.Faction, Target.Faction))
            {
                return false;
            }
            var currentBattle = GetCurrentBattle(Target, BattleKey.Create(Target.Position));
            if (currentBattle == null)
            {
                return true;
            }
            return CanParticipate(Formation, currentBattle.Battle, ReverseSide(currentBattle.Side));
        }

        public bool CanParticipate(IFormation Formation, Battle Battle, BattleSideType Side)
        {
            if (!Battle.GetFormations(Side)
                .Select(x => x.Faction)
                .Distinct()
                .All(x => x == Formation.Faction || !DiplomaticRelationGraph.CanAttack(Formation.Faction, x)))
            {
                return false;
            }
            if (!Battle.GetFormations(ReverseSide(Side))
                .Select(x => x.Faction)
                .Distinct()
                .All(x => x != Formation.Faction && DiplomaticRelationGraph.CanAttack(Formation.Faction, x)))
            {
                return false;
            }
            return true;
        }
        
        public void Engage(IFormation Formation, IFormation Target)
        {
            Formation.EnterCombat();
            var key = BattleKey.Create(Target.Position);
            var currentBattle = GetCurrentBattle(Target, key);
            if (currentBattle == null)
            {
                Target.EnterCombat();
                var newBattle = new Battle();
                newBattle.Add(Formation, BattleSideType.OFFENSE);
                newBattle.Add(Target, BattleSideType.DEFENSE);
                _ActiveBattles.Add(key, newBattle);
            }
            else
            {
                currentBattle.Battle.Add(Formation, ReverseSide(currentBattle.Side));
            }
        }

        public void Tick(Random Random)
        {
            var newActiveBattles = new MultiMap<BattleKey, Battle>();
            foreach (var battlesAndKey in _ActiveBattles)
            {
                foreach (var battle in battlesAndKey.Value.ToList())
                {
                    battle.Tick(Random);
                    foreach (var formation in battle.GetFormations(BattleSideType.OFFENSE).ToList())
                    {
                        if (formation.Cohesion.IsEmpty())
                        {
                            Disengage(formation, battle, BattleSideType.OFFENSE);
                        }
                    }
                    foreach (var formation in battle.GetFormations(BattleSideType.DEFENSE).ToList())
                    {
                        if (formation.Cohesion.IsEmpty())
                        {
                            Disengage(formation, battle, BattleSideType.DEFENSE);
                        }
                    }
                    if (battle.GetFormations(BattleSideType.OFFENSE).Count() == 0
                        || battle.GetFormations(BattleSideType.DEFENSE).Count() == 0)
                    {
                        foreach (var formation in battle.GetFormations(BattleSideType.OFFENSE))
                        {
                            formation.ExitCombat();
                        }
                        foreach (var formation in battle.GetFormations(BattleSideType.DEFENSE))
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
            _ActiveBattles = newActiveBattles;
        }

        private void Disengage(IFormation Formation, Battle Battle, BattleSideType Side)
        {
            Formation.ExitCombat();
            Battle.Remove(Formation, Side);
        }

        private BattleParticipation GetCurrentBattle(IFormation Formation, BattleKey Key)
        {
            if (!Formation.InCombat)
            {
                return null;
            }
            foreach (var battle in _ActiveBattles[Key])
            {
                var side = battle.GetBattleSide(Formation);
                switch (side)
                {
                    case BattleSideType.NONE:
                        continue;
                    case BattleSideType.OFFENSE:
                    case BattleSideType.DEFENSE:
                        return new BattleParticipation(battle, side);
                    default:
                        throw new InvalidProgramException();
                }
            }
            return null;
        }

        private BattleSideType ReverseSide(BattleSideType Side)
        {
            switch (Side)
            {
                case BattleSideType.OFFENSE:
                    return BattleSideType.DEFENSE;
                case BattleSideType.DEFENSE:
                    return BattleSideType.OFFENSE;
                default:
                    return BattleSideType.NONE;
            }
        }
    }
}