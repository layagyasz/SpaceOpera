using Cardamom.Trackers;
using SpaceOpera.Core.Designs;

namespace SpaceOpera.Core.Military
{
    public interface IFormationTemplate : IComponent
    {
        MultiCount<Unit> Composition { get; }
    }
}
