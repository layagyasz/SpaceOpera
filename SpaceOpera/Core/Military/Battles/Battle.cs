using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military.Battles
{
    class Battle
    {
        private readonly BattleReport.Builder _Report = new BattleReport.Builder();
        private readonly BattleSide _Offense = new BattleSide();
        private readonly BattleSide _Defense = new BattleSide();

        public void Add(IFormation Formation, BattleSideType Side)
        {
            switch (Side)
            {
                case BattleSideType.OFFENSE:
                    lock (_Offense)
                    {
                        _Offense.Add(Formation);
                    }
                    break;
                case BattleSideType.DEFENSE:
                    lock (_Defense)
                    {
                        _Defense.Add(Formation);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }

            var factionReport = _Report.GetBuilderFor(Formation.Faction);
            foreach (var group in Formation.Composition)
            {
                factionReport.GetBuilderFor(group.Unit).Add(group.Count);
            }
        }

        public BattleSideType GetBattleSide(IFormation Formation)
        {
            if (_Offense.Formations.Contains(Formation))
            {
                return BattleSideType.OFFENSE;
            }
            if (_Defense.Formations.Contains(Formation))
            {
                return BattleSideType.DEFENSE;
            }
            return BattleSideType.NONE;
        }

        public IEnumerable<IFormation> GetFormations(BattleSideType Type)
        {
            switch (Type)
            {
                case BattleSideType.OFFENSE:
                    return _Offense.Formations;
                case BattleSideType.DEFENSE:
                    return _Defense.Formations;
                default:
                    throw new ArgumentException();
            }
        }

        public BattleReport GetReport()
        {
            return _Report.Build();
        }

        public void Remove(IFormation Formation, BattleSideType Side)
        {
            switch (Side)
            {
                case BattleSideType.OFFENSE:
                    lock (_Offense)
                    {
                        _Offense.Remove(Formation);
                    }
                    return;
                case BattleSideType.DEFENSE:
                    lock (_Defense)
                    {
                        _Defense.Remove(Formation);
                    }
                    return;
                default:
                    throw new ArgumentException();
            }
        }

        public void Tick(Random Random)
        {
            lock (_Offense)
            {
                lock (_Defense)
                {
                    var defenses = _Defense.GetAttacks(_Offense, Random);
                    var attacks = _Offense.GetAttacks(_Defense, Random);
                    _Defense.Damage(attacks, _Report);
                    _Offense.Damage(defenses, _Report);
                }
            }
        }
    }
}