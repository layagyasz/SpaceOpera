using Cardamom.Ui;
using Cardamom.Ui.Controller;
using Cardamom.Ui.Controller.Element;
using SpaceOpera.Core;
using SpaceOpera.Core.Politics.Cultures;
using SpaceOpera.Core.Politics.Generator;
using SpaceOpera.View.GameSetup;

namespace SpaceOpera.Controller.GameSetup
{
    public class GameSetupFormController : IController
    {
        public EventHandler<EventArgs>? Started { get; set; }

        private readonly FactionGenerator _factionGenerator;
        private readonly Random _random;

        private GameSetupForm? _component;
        private BannerComponentController? _banner;
        private CultureComponentController? _culture;
        private GalaxyComponentController? _galaxy;
        private GovernmentComponentController? _government;
        private PoliticsComponentController? _politics;
        private IElementController? _start;

        public GameSetupFormController(FactionGenerator factionGenerator, Random random)
        {
            _factionGenerator = factionGenerator;
            _random = random;
        }

        public void Bind(object @object)
        {
            _component = (GameSetupForm)@object;
            _banner = (BannerComponentController)_component.Banner.ComponentController;
            _culture = (CultureComponentController)_component.Culture.ComponentController;
            _culture.ValueChanged += HandleCultureChanged;
            _galaxy = (GalaxyComponentController)_component.Galaxy.ComponentController;
            _government = (GovernmentComponentController)_component.Government.ComponentController;
            _politics = (PoliticsComponentController)_component.Politics.ComponentController;
            _start = _component.Start.Controller;

            _banner.Randomize(_random);
            _culture.Randomize(_random);
            _government.Randomize(_random);
            _start.Clicked += HandleStart;
        }

        public void Unbind()
        {
            _component = null;
            _banner = null;
            _culture!.ValueChanged -= HandleCultureChanged;
            _culture = null;
            _galaxy = null;
            _government = null;
            _politics = null;
            _start!.Clicked -= HandleStart;
            _start = null;
        }

        public GameParameters GetGameParameters()
        {
            var government = _government!.GetValue();
            var culture = new Culture(_culture!.GetValue(), government.NameGenerator.Language);
            return new(
                new()
                {
                    Galaxy = _galaxy!.GetValue(),
                    Politics = _politics!.GetValue()
                },
                _factionGenerator.Generate(
                    government.Name, 
                    _banner!.GetValue(),
                    culture,
                    government.Government!,
                    government.NameGenerator));
        }

        private void HandleCultureChanged(object? sender, EventArgs e)
        {
            _government!.SetCulture(_culture!.GetValue());
        }

        private void HandleStart(object? sender,  MouseButtonClickEventArgs e)
        {
            if (_government!.GetValue().Government != null)
            {
                Started?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
