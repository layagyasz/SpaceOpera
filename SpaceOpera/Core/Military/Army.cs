using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public class Army : IFormation
    {
        public string Name { get; private set; } = string.Empty;
        public Faction Faction { get; }

        private readonly List<Division> _divisions = new();

        public Army(Faction faction)
        {
            Faction = faction;
        }

        public void Add(Division division)
        {
            _divisions.Add(division);
        }

        public void CheckDivisions()
        {
            lock (_divisions)
            {
                _divisions.RemoveAll(x => x.IsDestroyed());
            }
        }

        public bool IsDestroyed()
        {
            return !_divisions.Any();
        }

        public void Remove(Division division)
        {
            _divisions.Remove(division);
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}
