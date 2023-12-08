using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Military
{
    public interface IFormation
    {
        string Name { get; }
        Faction Faction { get; }
        float GetMilitaryPower();
        bool IsDestroyed();
        void SetName(string name);
    }
}
