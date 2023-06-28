using Cardamom.Ui.Controller;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class GameSetupFormController : IController
    {
        private readonly Random _random;

        private GameSetupForm? _component;
        private BannerComponentController? _banner;
        private CultureComponentController? _culture;
        private GalaxyComponentController? _galaxy;
        private GovernmentComponentController? _government;
        private PoliticsComponentController? _politics;

        public GameSetupFormController(Random random)
        {
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (GameSetupForm)@object;
            _banner = (BannerComponentController)_component.Banner.ComponentController;
            _culture = (CultureComponentController)_component.Culture.ComponentController;
            _galaxy = (GalaxyComponentController)_component.Galaxy.ComponentController;
            _government = (GovernmentComponentController)_component.Government.ComponentController;
            _politics = (PoliticsComponentController)_component.Politics.ComponentController;

            _banner.Randomize(_random);
            _culture.Randomize(_random);
            _government.Randomize(_random);
        }

        public void Unbind()
        {
            _component = null;
            _banner = null;
            _culture = null;
            _galaxy = null;
            _government = null;
            _politics = null;
        }
    }
}
