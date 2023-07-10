using Cardamom.Graphics;
using Cardamom.Ui.Controller;

namespace SpaceOpera.View
{
    public interface IScreen : IRenderable, IDisposable
    {
        IController Controller { get; }
    }
}
