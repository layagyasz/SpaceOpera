using SpaceOpera.Core;
using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera
{
    public class World
    {
        public GameData GameData { get; }
        public Random Random { get; }
        public StarCalendar Calendar { get; }
        public Galaxy Galaxy { get; }
        public NavigationMap NavigationMap { get; }
        public DiplomaticRelationGraph DiplomaticRelations { get; }
        public Economy Economy { get; }
        public EconomyGraph EconomyGraph { get; }
        public BattleManager BattleManager { get; }
        public DesignBuilder DesignBuilder { get; }
        public AutoDesigner AutoDesigner { get; }

        private readonly List<Culture> _cultures = new();
        private readonly List<Faction> _factions = new();
        private readonly Dictionary<Faction, Intelligence> _intelligence = new();

        private readonly List<Design> _designs = new();
        private readonly List<DesignLicense> _designLicenses = new();

        private readonly List<Fleet> _fleets = new();
        private readonly FleetManager _fleetManager = new();

        private readonly ProjectManager _projectManager = new();

        private World(
            GameData gameData, 
            Random random,
            Galaxy galaxy,
            StarCalendar calendar)
        {
            Calendar = calendar;
            GameData = gameData;
            Random = random;
            Galaxy = galaxy;
            NavigationMap = new(galaxy);
            DiplomaticRelations = new();
            Economy = new(gameData.MaterialSink!);
            EconomyGraph = new();
            EconomyGraph.AddRecipes(gameData.Recipes.Values);
            BattleManager = new(DiplomaticRelations);
            DesignBuilder = new(new ComponentClassifier(gameData.ComponentClassifiers));
            AutoDesigner = new(gameData.DesignTemplates.Values);

            foreach (var material in gameData.Materials)
            {
                Console.WriteLine("[{0}]", material.Key);
                foreach (var recipe in EconomyGraph.GetRequiredRecipes(material.Value).GetQuantities())
                {
                    Console.WriteLine("\t{0} * {1}", recipe.Key.Key, recipe.Value);
                }
            }
        }

        public static World Generate(Culture PlayerCulture, Faction PlayerFaction, GameData GameData, Random Random)
        {
            var world =
                new World(
                    GameData, 
                    Random,
                    GameData.GalaxyGenerator!.Generate(Random),
                    new StarCalendar(/* StartDate= */ 1440000));
            GameData.PoliticsGenerator!.Generate(world, PlayerCulture, PlayerFaction, Random);
            GameData.EconomyGenerator!.Generate(world, Random);
            return world;
        }

        public ITickable GetTickable()
        {
            var ticks = new List<ITickable>
            {
                Economy
            };
            ticks.AddRange(_factions);
            return new CompositeTickable() {
                Calendar,
                new ActionTickable(() => BattleManager.Tick(Random)),
                new ActionTickable(() => _fleetManager.Tick(this)),
                new ActionTickable(_projectManager.Tick),
                new CycleTickable(new CompositeTickable(ticks), 30)
            };
        }

        public void AddAllCultures(IEnumerable<Culture> Cultures)
        {
            _cultures.AddRange(Cultures);
        }

        public void AddAllFactions(IEnumerable<Faction> Factions)
        {
            _factions.AddRange(Factions);
            DiplomaticRelations.Initialize(Factions);
            foreach (var faction in Factions)
            {
                _intelligence.Add(faction, new());
            }
        }

        public void AddDesign(Design Design)
        {
            _designs.Add(Design);
            foreach (var recipe in Design.Recipes)
            {
                EconomyGraph.AddRecipe(recipe);
            }
        }

        public void AddFleet(Fleet Fleet)
        {
            _fleets.Add(Fleet);
            _fleetManager.AddFleet(Fleet);
        }

        public void AddLicense(DesignLicense License)
        {
            _designLicenses.Add(License);
        }

        public void AddProject(IProject project)
        {
            _projectManager.Add(project);
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
            return _designLicenses.Where(x => x.Faction == Faction).Select(x => x.Design);
        }

        public FleetDriver GetDriver(Fleet Fleet)
        {
            return _fleetManager.GetDriver(Fleet);
        }

        public IEnumerable<Faction> GetFactions()
        {
            return _factions;
        }

        public IEnumerable<Fleet> GetFleets()
        {
            return _fleets;
        }

        public IEnumerable<Fleet> GetFleetsFor(Faction Faction)
        {
            return _fleets.Where(x => x.Faction == Faction);
        }

        public Intelligence GetIntelligenceFor(Faction Faction)
        {
            return _intelligence[Faction];
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
                _designLicenses.Where(x => x.Faction == Faction).SelectMany(x => x.Design.Recipes),
                GameData.Recipes.Values)
                .Where(Faction.HasPrerequisiteResearch);
        }

        public IEnumerable<Structure> GetStructures()
        {
            return GameData.Structures.Values;
        }
    }
}