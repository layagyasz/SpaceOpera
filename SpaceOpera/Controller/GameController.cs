using Cardamom.Logging;
using Cardamom.Ui;
using Cardamom.Ui.Controller;

namespace SpaceOpera.Controller
{
    public class GameController : IController
    {
        private readonly ILogger _logger;
        private Screen? _screen;

        public GameController(ILogger logger)
        {
            _logger = logger;
        }

        public void Bind(object @object)
        {
            _screen = @object as Screen;
        }

        public void Unbind()
        {
            _screen = null;
        }

        public void HandleInteraction(UiInteractionEventArgs e)
        {
            _logger.AtInfo().Log(e.ToString());
        }
    }
}
