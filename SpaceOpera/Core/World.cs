using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.Core.Orders;
using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Politics.Cultures;
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
        public AdvancementManager AdvancementManager { get; } = new();
        public Economy Economy { get; }
        public EconomyGraph EconomyGraph { get; } = new();
        public FormationManager FormationManager { get; } = new();
        public BattleManager BattleManager { get; }
        public ProjectManager ProjectManager { get; } = new();
        public DesignBuilder DesignBuilder { get; }
        public AutoDesigner AutoDesigner { get; }

        private readonly List<Culture> _cultures = new();
        private readonly List<Faction> _factions = new();
        private readonly Dictionary<Faction, Intelligence> _intelligence = new();

        private readonly List<Design> _designs = new();
        private readonly List<DesignLicense> _designLicenses = new();

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
            AdvancementManager = new();
            Economy = new(AdvancementManager, FormationManager, coreData.MaterialSink!);
            EconomyGraph.AddRecipes(coreData.Recipes.Values);
            BattleManager = new(DiplomaticRelations);
            DesignBuilder = new(new ComponentClassifier(coreData.ComponentClassifiers));
            AutoDesigner = new(coreData.DesignTemplates.Values);
        }

        public ValidationFailureReason Execute(IOrder order)
        {
            var validation = order.Validate(this);
            if (validation != ValidationFailureReason.None)
            {
                return validation;
            }
            order.Execute(this);
            return ValidationFailureReason.None;
        }

        public IUpdateable GetUpdater()
        {
            var ticks = new List<ITickable>
            {
                AdvancementManager,
                Economy
            };

            return new CompositeUpdateable()
            {
                Calendar,
                new TickUpdateable(
                    new CompositeTickable() {
                        new ActionTickable(() => BattleManager.Tick(Random)),
                        new ActionTickable(() => FormationManager.Tick(this)),
                        new ActionTickable(ProjectManager.Tick),
                        new CycleTickable(new CompositeTickable(ticks), 30)
                    },
                    1000)
            };
        }

        public void AddAllCultures(IEnumerable<Culture> cultures)
        {
            _cultures.AddRange(cultures);
        }

        public void AddAllFactions(IEnumerable<Faction> factions)
        {
            _factions.AddRange(factions);
            DiplomaticRelations.AddAllFactions(factions);
            foreach (var faction in factions)
            {
                AdvancementManager.Add(faction);
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

        public void AddLicense(DesignLicense license)
        {
            _designLicenses.Add(license);
        }

        public IEnumerable<IComponent> GetComponentsFor(Faction faction)
        {
            if (faction == null)
            {
                return Enumerable.Empty<IComponent>();
            }
            var advancements = AdvancementManager.Get(faction);
            return GetDesignsFor(faction).SelectMany(x => x.Components).Cast<IComponent>().Concat(
                CoreData.Components.Values).Where(advancements.HasPrerequisiteResearch);
        }

        public IEnumerable<Design> GetDesignsFor(Faction faction)
        {
            if (faction == null)
            {
                return Enumerable.Empty<Design>();
            }
            return _designLicenses.Where(x => x.Faction == faction).Select(x => x.Design);
        }

        public IEnumerable<Faction> GetFactions()
        {
            return _factions;
        }

        public Intelligence GetIntelligenceFor(Faction faction)
        {
            return _intelligence[faction];
        }

        public IEnumerable<IAdvancement> GetResearchableAdvancementsFor(Faction faction)
        {
            var advancements = AdvancementManager.Get(faction);
            return CoreData.Advancements.Select(x => x.Value).Where(advancements.HasPrerequisiteResearch);
        }

        public IEnumerable<Recipe> GetRecipesFor(Faction? faction)
        {
            if (faction == null)
            {
                return Enumerable.Empty<Recipe>();
            }
            var advancements = AdvancementManager.Get(faction);
            return _designLicenses.Where(x => x.Faction == faction).SelectMany(x => x.Design.Recipes).Concat(
                CoreData.Recipes.Values)
                .Where(advancements.HasPrerequisiteResearch);
        }

        public IEnumerable<Structure> GetStructures()
        {
            return CoreData.Structures.Values;
        }
    }
}