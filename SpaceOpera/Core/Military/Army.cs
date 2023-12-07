using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public class Army : IFormation
    {
        public string Name { get; private set; } = string.Empty;
        public Faction Faction { get; }

        private readonly List<Division> _divisions = new();
        private bool _isDestructable;

        public Army(Faction faction)
        {
            Faction = faction;
        }

        public void Add(Division division)
        {
            _divisions.Add(division);
            _isDestructable = true;
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
            return _isDestructable && !_divisions.Any();
        }

        public void Remove(Division division)
        {
            _divisions.Remove(division);
            if (!_divisions.Any())
            {
                _isDestructable = false;
            }
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}
