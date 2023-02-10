using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.Core
{
    public class World
    {
        public CoreData CoreData { get; }
        public Random Random { get; }
        public StarCalendar Calendar { get; }
        public Galaxy Galaxy { get; }
        public NavigationMap NavigationMap { get; }
        public DiplomaticRelationGraph DiplomaticRelations { get; } = new();
        public Economy Economy { get; }
        public EconomyGraph EconomyGraph { get; } = new();
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

        public World(
            CoreData coreData,
            Random random,
            Galaxy galaxy,
            StarCalendar calendar,
            NavigationMap navigationMap)
        {
            Calendar = calendar;
            CoreData = coreData;
            Random = random;
            Galaxy = galaxy;
            NavigationMap = navigationMap;
            Economy = new(coreData.MaterialSink!);
            EconomyGraph.AddRecipes(coreData.Recipes.Values);
            BattleManager = new(DiplomaticRelations);
            DesignBuilder = new(new ComponentClassifier(coreData.ComponentClassifiers));
            AutoDesigner = new(coreData.DesignTemplates.Values);

            foreach (var material in coreData.Materials)
            {
                Console.WriteLine("[{0}]", material.Key);
                foreach (var recipe in EconomyGraph.GetRequiredRecipes(material.Value).GetQuantities())
                {
                    Console.WriteLine("\t{0} * {1}", recipe.Key.Key, recipe.Value);
                }
            }
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

        public void AddAllCultures(IEnumerable<Culture> cultures)
        {
            _cultures.AddRange(cultures);
        }

        public void AddAllFactions(IEnumerable<Faction> factions)
        {
            _factions.AddRange(factions);
            DiplomaticRelations.Initialize(factions);
            foreach (var faction in factions)
            {
                _intelligence.Add(faction, new());
            }
        }

        public void AddDesign(Design design)
        {
            _designs.Add(design);
            foreach (var recipe in design.Recipes)
            {
                EconomyGraph.AddRecipe(recipe);
            }
        }

        public void AddFleet(Fleet fleet)
        {
            _fleets.Add(fleet);
            _fleetManager.AddFleet(fleet);
        }

        public void AddLicense(DesignLicense license)
        {
            _designLicenses.Add(license);
        }

        public void AddProject(IProject project)
        {
            _projectManager.Add(project);
        }

        public IEnumerable<IComponent> GetComponentsFor(Faction faction)
        {
            if (faction == null)
            {
                return Enumerable.Empty<IComponent>();
            }
            return GetDesignsFor(faction).SelectMany(x => x.Components).Cast<IComponent>().Concat(
                CoreData.Components.Values).Where(faction.HasPrerequisiteResearch);
        }

        public IEnumerable<Design> GetDesignsFor(Faction faction)
        {
            if (faction == null)
            {
                return Enumerable.Empty<Design>();
            }
            return _designLicenses.Where(x => x.Faction == faction).Select(x => x.Design);
        }

        public FleetDriver GetDriver(Fleet fleet)
        {
            return _fleetManager.GetDriver(fleet);
        }

        public IEnumerable<Faction> GetFactions()
        {
            return _factions;
        }

        public IEnumerable<Fleet> GetFleets()
        {
            return _fleets;
        }

        public IEnumerable<Fleet> GetFleetsFor(Faction faction)
        {
            return _fleets.Where(x => x.Faction == faction);
        }

        public Intelligence GetIntelligenceFor(Faction faction)
        {
            return _intelligence[faction];
        }

        public IEnumerable<IAdvancement> GetResearchableAdvancementsFor(Faction faction)
        {
            return CoreData.Advancements.Select(x => x.Value).Where(faction.HasPrerequisiteResearch);
        }

        public IEnumerable<Recipe> GetRecipesFor(Faction faction)
        {
            if (faction == null)
            {
                return Enumerable.Empty<Recipe>();
            }
            return _designLicenses.Where(x => x.Faction == faction).SelectMany(x => x.Design.Recipes).Concat(
                CoreData.Recipes.Values)
                .Where(faction.HasPrerequisiteResearch);
        }

        public IEnumerable<Structure> GetStructures()
        {
            return CoreData.Structures.Values;
        }
    }
}