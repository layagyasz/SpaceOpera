using Cardamom.Collections;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System.Linq;

namespace SpaceOpera.Core.Military.Battles
{
    public class BattleManager
    {
        record class BattleKey(INavigable Position);

        public DiplomaticRelations DiplomaticRelations { get; }
        public FormationManager Formations { get; }

        private MultiMap<BattleKey, Battle> _activeBattles = new();

        public BattleManager(DiplomaticRelations diplomaticRelations, FormationManager formations)
        {
            DiplomaticRelations = diplomaticRelations;
            Formations = formations;
        }

        public void Attack(IAtomicFormation formation, INavigable location)
        {
            var battle = 
                _activeBattles[new(location)]
                    .FirstOrDefault(x => CanParticipate(formation, x, BattleSideType.Offense));
            if (battle == null)
            {
                battle = new(location);
                _activeBattles.Add(new(location), battle);
                foreach (var other in Formations.GetFormationsIn(location))
                {
                    if (CanParticipate(other.AtomicFormation, battle, BattleSideType.Defense))
                    {
                        Join(other.AtomicFormation, battle, BattleSideType.Defense);
                    }
                }
            }
            Join(formation, battle, BattleSideType.Offense);
        }

        public bool CanAttack(IAtomicFormation formation, INavigable location)
        {
            return Formations.GetFormationsIn(location)
                .Select(x => x.Formation.Faction)
                .Distinct()
                .Any(x => x != formation.Faction && DiplomaticRelations.CanAttack(formation.Faction, x));
        }

        public bool CanDefend(IAtomicFormation formation)
        {
            return _activeBattles[new(formation.Position!)]
                .Any(x => !x.Contains(formation) && CanParticipate(formation, x, BattleSideType.Defense));
        }

        public bool CanEngage(IAtomicFormation formation, IAtomicFormation target)
        {
            if (!DiplomaticRelations.CanAttack(formation.Faction, target.Faction))
            {
                return false;
            }
            var currentBattle = GetCurrentBattle(target, new(target.Position!));
            if (currentBattle == null)
            {
                return true;
            }
            return CanParticipate(formation, currentBattle.Battle, ReverseSide(currentBattle.Side));
        }

        public bool CanParticipate(IAtomicFormation formation, Battle battle, BattleSideType side)
        {
            if (side == BattleSideType.Defense && formation.Position != battle.Location)
            {
                return false;
            }
            if (!battle.GetFormations(side)
                .Select(x => x.Faction)
                .Distinct()
                .All(x => x == formation.Faction || !DiplomaticRelations.CanAttack(formation.Faction, x)))
            {
                return false;
            }
            if (!battle.GetFormations(ReverseSide(side))
                .Select(x => x.Faction)
                .Distinct()
                .All(x => x != formation.Faction && DiplomaticRelations.CanAttack(formation.Faction, x)))
            {
                return false;
            }
            return true;
        }

        public void Defend(IAtomicFormation formation)
        {
            foreach (var battle in _activeBattles[new(formation.Position!)])
            {
                if (!battle.Contains(formation) && CanParticipate(formation, battle, BattleSideType.Defense))
                {
                    Join(formation, battle, BattleSideType.Defense);
                }
            }
        }
        
        public void Engage(IAtomicFormation formation, IAtomicFormation target)
        {
            formation.EnterCombat();
            var key = new BattleKey(target.Position!);
            var currentBattle = GetCurrentBattle(target, key);
            if (currentBattle == null)
            {
                target.EnterCombat();
                var newBattle = new Battle(key.Position);
                newBattle.Add(formation, BattleSideType.Offense);
                newBattle.Add(target, BattleSideType.Defense);
                _activeBattles.Add(key, newBattle);
            }
            else
            {
                currentBattle.Battle.Add(formation, ReverseSide(currentBattle.Side));
            }
        }

        public bool IsDefending(IAtomicFormation formation)
        {
            return _activeBattles[new(formation.Position!)]
                .Any(x => x.GetBattleSide(formation) == BattleSideType.Defense);
        }

        public Battle? GetBattle(IAtomicFormation formation)
        {
            if (formation.InCombat == 0)
            {
                return null;
            }
            if (_activeBattles.TryGetValue(new(formation.Position!), out var battle))
            {
                return battle.FirstOrDefault(x => x.Contains(formation));
            }
            return _activeBattles.Values.SelectMany(x => x).FirstOrDefault(x => x.Contains(formation));
        }

        public void Tick(Random random)
        {
            var newActiveBattles = new MultiMap<BattleKey, Battle>();
            foreach (var battlesAndKey in _activeBattles)
            {
                foreach (var battle in battlesAndKey.Value.ToList())
                {
                    foreach (var formation in battle.GetFormations(BattleSideType.Offense).ToList())
                    {
                        if (formation.IsDestroyed())
                        {
                            battle.Remove(formation, BattleSideType.Offense);
                        }
                    }
                    foreach (var formation in battle.GetFormations(BattleSideType.Defense).ToList())
                    {
                        if (formation.IsDestroyed())
                        {
                            battle.Remove(formation, BattleSideType.Defense);
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
                        battle.Tick(random);
                        newActiveBattles.Add(battlesAndKey);
                    }
                }
            }
            _activeBattles = newActiveBattles;
        }

        public void Withdraw(IAtomicFormation formation)
        {
            foreach (var battle in _activeBattles.Values.SelectMany(x => x))
            {
                if (battle.Contains(formation))
                {
                    Leave(formation, battle);
                }
            }
        }

        private BattleParticipation? GetCurrentBattle(IAtomicFormation formation, BattleKey key)
        {
            if (formation.InCombat == 0)
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

        private static void Leave(IAtomicFormation formation, Battle battle)
        {
            var side = battle.GetBattleSide(formation);
            if (side != BattleSideType.None)
            {
                formation.ExitCombat();
                battle.Remove(formation, side);
            }
        }

        private static void Join(IAtomicFormation formation, Battle battle, BattleSideType side)
        {
            formation.EnterCombat();
            battle.Add(formation, side);
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