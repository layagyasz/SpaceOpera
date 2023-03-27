using Cardamom.Ui;
using SpaceOpera.View.Highlights;

namespace SpaceOpera.View.Scenes
{
    public interface IGameScene : IScene, IDisposable
    {
        void SetHighlight(HighlightLayerName layer, ICompositeHighlight? highlight);
    }
}
