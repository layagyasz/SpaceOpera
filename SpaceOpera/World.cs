using SpaceOpera.Core;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View;
using SpaceOpera.View.StellarBodyViews;

namespace SpaceOpera
{
    public class World
    {
        public GameData GameData { get; }
        public IconFactory IconFactory { get; }
        public Random Random { get; }
        public StarCalendar Calendar { get; }
        public Galaxy Galaxy { get; }
        public NavigationMap NavigationMap { get; }
        public DiplomaticRelationGraph DiplomaticRelations { get; }
        public Economy Economy { get; }
        public EconomyGraph EconomyGraph { get; }
        public BattleManager BattleManager { get; }

        private readonly List<Culture> _Cultures = new List<Culture>();
        private readonly List<Faction> _Factions = new List<Faction>();
        private readonly Dictionary<Faction, Intelligence> _Intelligence = new Dictionary<Faction, Intelligence>();

        private readonly List<Design> _Designs = new List<Design>();
        private readonly List<DesignLicense> _DesignLicenses = new List<DesignLicense>();

        private readonly List<Fleet> _Fleets = new List<Fleet>();

        private readonly FleetManager _FleetManager = new FleetManager();

        private World(
            GameData GameData, 
            Random Random,
            Galaxy Galaxy,
            StarCalendar Calendar)
        {
            this.Calendar = Calendar;
            this.GameData = GameData;
            this.IconFactory =
                new IconFactory(
                    GameData.SimpleIconFactory,
                    GameData.DesignedComponentIconFactory, 
                    GameData.BannerViewFactory,
                    new StellarBodyIconFactory(
                        Galaxy.Systems.SelectMany(x => x.Orbiters), 
                        GameData.StellarBodyViewFactory, 
                        /* SpriteSize= */ 64, 
                        /* TextureSize= */ 1024,
                        GameData.LightingShader));
            IconFactory.StellarBodyIconFactory.WriteToFile("stellar-bodies");
            this.Random = Random;
            this.Galaxy = Galaxy;
            this.NavigationMap = new NavigationMap(Galaxy);
            this.DiplomaticRelations = new DiplomaticRelationGraph();
            this.Economy = new Economy(GameData.MaterialSink);
            this.EconomyGraph = new EconomyGraph();
            this.EconomyGraph.AddRecipes(GameData.Recipes.Values);
            this.BattleManager = new BattleManager(DiplomaticRelations);

            foreach (var material in GameData.Materials)
            {
                Console.WriteLine("[{0}]", material.Key);
                foreach (var recipe in EconomyGraph.GetRequiredRecipes(material.Value).GetQuantities())
                {
                    Console.WriteLine("\t{0} * {1}", recipe.Value.Key, recipe.Amount);
                }
            }
        }

        public static World Generate(Culture PlayerCulture, Faction PlayerFaction, GameData GameData, Random Random)
        {
            var world =
                new World(
                    GameData, 
                    Random,
                    GameData.GalaxyGenerator.Generate(Random),
                    new StarCalendar(/* StartDate= */ 1440000));
            GameData.PoliticsGenerator.Generate(world, PlayerCulture, PlayerFaction, Random);
            GameData.EconomyGenerator.Generate(world, Random);
            return world;
        }

        public ITickable GetTickable()
        {
            var ticks = new List<ITickable>();
            ticks.Add(Economy);
            ticks.AddRange(_Factions);
            return new CompositeTickable() {
                Calendar,
                new ActionTickable(() => BattleManager.Tick(Random)),
                new ActionTickable(() => _FleetManager.Tick(this)),
                new CycleTickable(new CompositeTickable(ticks), 30)
            };
        }

        public void AddAllCultures(IEnumerable<Culture> Cultures)
        {
            _Cultures.AddRange(Cultures);
        }

        public void AddAllFactions(IEnumerable<Faction> Factions)
        {
            _Factions.AddRange(Factions);
            DiplomaticRelations.Initialize(Factions);
            foreach (var faction in Factions)
            {
                _Intelligence.Add(faction, new Intelligence());
            }
        }

        public void AddDesign(Design Design)
        {
            _Designs.Add(Design);
            foreach (var recipe in Design.Recipes)
            {
                EconomyGraph.AddRecipe(recipe);
            }
        }

        public void AddFleet(Fleet Fleet)
        {
            _Fleets.Add(Fleet);
            _FleetManager.AddFleet(Fleet);
        }

        public void AddLicense(DesignLicense License)
        {
            _DesignLicenses.Add(License);
        }

        public IEnumerable<IComponent> GetComponentsFor(Faction Faction)
        {
            if (Faction == null)
            {
                return Enumerable.Empty<IComponent>();
            }
            return Enumerable.Concat(
                GetDesignsFor(Faction).SelectMany(x => x.Components).Cast<IComponent>(), 
                GameData.Components.Values).Where(Faction.HasPrerequisiteResearch);
        }

        public IEnumerable<Design> GetDesignsFor(Faction Faction)
        {
            if (Faction == null)
            {
                return Enumerable.Empty<Design>();
            }
            return _DesignLicenses.Where(x => x.Faction == Faction).Select(x => x.Design);
        }

        public FleetDriver GetDriver(Fleet Fleet)
        {
            return _FleetManager.GetDriver(Fleet);
        }

        public IEnumerable<Faction> GetFactions()
        {
            return _Factions;
        }

        public IEnumerable<Fleet> GetFleets()
        {
            return _Fleets;
        }

        public IEnumerable<Fleet> GetFleetsFor(Faction Faction)
        {
            return _Fleets.Where(x => x.Faction == Faction);
        }

        public Intelligence GetIntelligenceFor(Faction Faction)
        {
            return _Intelligence[Faction];
        }

        public IEnumerable<IAdvancement> GetResearchableAdvancementsFor(Faction Faction)
        {
            return GameData.Advancements.Select(x => x.Value).Where(Faction.HasPrerequisiteResearch);
        }

        public IEnumerable<Recipe> GetRecipesFor(Faction Faction)
        {
            if (Faction == null)
            {
                return Enumerable.Empty<Recipe>();
            }
            return Enumerable.Concat(
                _DesignLicenses.Where(x => x.Faction == Faction).SelectMany(x => x.Design.Recipes),
                GameData.Recipes.Values)
                .Where(Faction.HasPrerequisiteResearch);
        }

        public IEnumerable<Structure> GetStructures()
        {
            return GameData.Structures.Values;
        }
    }
}