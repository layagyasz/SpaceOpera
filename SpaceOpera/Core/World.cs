using SpaceOpera.Core.Advanceable;
using SpaceOpera.Core.Advancement;
using SpaceOpera.Core.Designs;
using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Economics.Projects;
using SpaceOpera.Core.Events;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Military.Battles;
using SpaceOpera.Core.Military.Fronts;
using SpaceOpera.Core.Military.Intelligence;
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

        public AdvancementManager Advancements { get; }
        public BattleManager Battles { get; }
        public DiplomaticRelations DiplomaticRelations { get; } = new();
        public Economy Economy { get; }
        public EconomyGraph EconomyGraph { get; } = new();
        public EventManager Events { get; } = new();
        public FormationManager Formations { get; } = new();
        public IntelligenceManager Intelligence { get; } = new();
        public FrontManager Fronts { get; }
        public ProjectManager Projects { get; } = new();

        public DesignBuilder DesignBuilder { get; }
        public AutoDesigner AutoDesigner { get; }

        public PlayerManager Players { get; } = new();

        private readonly List<Culture> _cultures = new();
        private readonly List<Faction> _factions = new();

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

            Advancements = new(coreData.Materials.Values.Where(x => x.Type == MaterialType.Research), random);
            Battles = new(DiplomaticRelations, Formations);
            Economy = new(Advancements, Formations, coreData.MaterialSink!);
            EconomyGraph.AddRecipes(coreData.Recipes.Values);
            Fronts = FrontManager.Create(galaxy);

            DesignBuilder = new(new ComponentClassifier(coreData.ComponentClassifiers));
            AutoDesigner = new(coreData.DesignTemplates.Values);

            foreach (var advancement in coreData.Advancements.Values)
            {
                Advancements.AddAdvancement(advancement);
            }
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

        public float GetPower(Faction faction)
        {
            return Formations.GetGroundForcePower(faction) + Formations.GetFleetPower(faction);
        }

        public IUpdateable GetUpdater()
        {
            var ticks = new List<ITickable>
            {
                Advancements,
                Economy
            };

            return new CompositeUpdateable()
            {
                Calendar,
                new TickUpdateable(
                    new CompositeTickable() {
                        new ActionTickable(() => Battles.Tick(Random)),
                        new ActionTickable(() => Formations.Tick(this)),
                        new ActionTickable(Fronts.Tick),
                        new ActionTickable(() => Projects.Tick(this)),
                        new CycleTickable(new CompositeTickable(ticks), 30),
                        new ActionTickable(() => Players.Tick(this))
                    },
                    1000)
            };
        }

        public void AddAllCultures(IEnumerable<Culture> cultures)
        {
            _cultures.AddRange(cultures);
        }

        public void AddAllFactions(Faction playerFaction, IEnumerable<Faction> factions)
        {
            _factions.Add(playerFaction);
            Advancements.AddFaction(playerFaction);
            DiplomaticRelations.Add(playerFaction);
            Economy.Add(playerFaction);
            Intelligence.Add(playerFaction);
            Players.Add(playerFaction, /* isHuman= */ true);

            _factions.AddRange(factions);
            foreach (var faction in factions)
            {
                Advancements.AddFaction(faction);
                DiplomaticRelations.Add(faction);
                Economy.Add(faction);
                Intelligence.Add(faction);
                Players.Add(faction, /* isHuman= */ false);
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
            var advancements = Advancements.Get(faction);
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

        public IEnumerable<Recipe> GetRecipesFor(Faction? faction)
        {
            if (faction == null)
            {
                return Enumerable.Empty<Recipe>();
            }
            var advancements = Advancements.Get(faction);
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