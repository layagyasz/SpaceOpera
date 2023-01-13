namespace SpaceOpera.Core.Military.Battles
{
    public class BattleParticipation
    {
        public Battle Battle { get; }
        public BattleSideType Side { get; }

        public BattleParticipation(Battle battle, BattleSideType side)
        {
            Battle = battle;
            Side = side;
        }
    }
}
