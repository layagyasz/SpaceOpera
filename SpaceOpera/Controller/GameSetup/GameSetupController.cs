using Cardamom.Ui.Controller;
using SpaceOpera.Core;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class GameSetupController : IController
    {
        public EventHandler<GameParameters>? GameStarted { get; set; }

        private GameSetupScreen? _screen;
        private GameSetupFormController? _form;

        public void Bind(object @object)
        {
            _screen = (GameSetupScreen)@object;
            _form = (GameSetupFormController)_screen.GetForm().ComponentController;
            _form.Started += HandleGameStarted;
        }

        public void Unbind()
        {
            _screen = null;
            _form!.Started -= HandleGameStarted;
            _form = null;
        }

        private void HandleGameStarted(object? sender, EventArgs e)
        {
            GameStarted?.Invoke(this, _form!.GetGameParameters());
        }
    }
}
