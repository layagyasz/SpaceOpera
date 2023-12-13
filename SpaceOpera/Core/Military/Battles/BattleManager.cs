using Cardamom.Collections;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

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
                battle = new(location, isMandatory: true);
                _activeBattles.Add(new(location), battle);
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
        
        public void Engage(IAtomicFormation formation, IAtomicFormation target)
        {
            formation.EnterCombat();
            var key = new BattleKey(target.Position!);
            var currentBattle = GetCurrentBattle(target, key);
            if (currentBattle == null)
            {
                target.EnterCombat();
                var newBattle = new Battle(key.Position, isMandatory: false);
                newBattle.Add(formation, BattleSideType.Offense);
                newBattle.Add(target, BattleSideType.Defense);
                _activeBattles.Add(key, newBattle);
            }
            else
            {
                currentBattle.Battle.Add(formation, ReverseSide(currentBattle.Side));
            }
        }

        public Battle? GetBattle(IAtomicFormation formation)
        {
            if (!formation.InCombat)
            {
                return null;
            }
            if (_activeBattles.TryGetValue(new(formation.Position!), out var battle))
            {
                return battle.FirstOrDefault(x => x.Contains(formation));
            }
            return null;
        }

        public void Tick(Random random)
        {
            var newActiveBattles = new MultiMap<BattleKey, Battle>();
            foreach (var battlesAndKey in _activeBattles)
            {
                foreach (var battle in battlesAndKey.Value.ToList())
                {
                    if (battle.IsMandatory)
                    {
                        foreach (var formation in Formations.GetFormationsIn(battle.Location))
                        {
                            if (!battle.Contains(formation.AtomicFormation)
                                && CanParticipate(formation.AtomicFormation, battle, BattleSideType.Defense))
                            {
                                Join(formation.AtomicFormation, battle, BattleSideType.Offense);
                            }
                        }
                    }
                    battle.Tick(random);
                    foreach (var formation in battle.GetFormations(BattleSideType.Offense).ToList())
                    {
                        formation.CheckInventory();
                        if (formation.Cohesion.IsEmpty())
                        {
                            Leave(formation, battle, BattleSideType.Offense);
                        }
                    }
                    foreach (var formation in battle.GetFormations(BattleSideType.Defense).ToList())
                    {
                        formation.CheckInventory();
                        if (formation.Cohesion.IsEmpty())
                        {
                            Leave(formation, battle, BattleSideType.Defense);
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
                }
            }
            _activeBattles = newActiveBattles;
        }

        private BattleParticipation? GetCurrentBattle(IAtomicFormation formation, BattleKey key)
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

        private static void Leave(IAtomicFormation formation, Battle battle, BattleSideType side)
        {
            formation.ExitCombat();
            battle.Remove(formation, side);
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