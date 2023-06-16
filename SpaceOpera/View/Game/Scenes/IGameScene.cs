using Cardamom.Ui;
using SpaceOpera.View.Game.Highlights;

namespace SpaceOpera.View.Game.Scenes
{
    public interface IGameScene : IScene, IDisposable, IDynamic
    {
        void SetHighlight(HighlightLayerName layer, ICompositeHighlight? highlight);
    }
}
