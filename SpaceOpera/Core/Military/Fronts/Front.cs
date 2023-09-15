using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military.Fronts
{
    public class Front
    {
        public Faction Faction { get; }
        public Faction? Opponent { get; }
        public List<FrontEdge> Edges { get; }

        public Front(Faction faction, Faction? opponent, List<FrontEdge> edges)
        {
            Faction = faction;
            Opponent = opponent;
            Edges = edges;
        }
    }
}
