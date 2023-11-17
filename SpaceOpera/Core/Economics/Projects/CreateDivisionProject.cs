using SpaceOpera.Core.Military;
using SpaceOpera.Core.Politics;

namespace SpaceOpera.Core.Economics.Projects
{
    public class CreateDivisionProject : BaseResourcedProject
    {
        public override object Key => Template;
        public override string Name => $"Create {DivisionName}";
        public Faction Faction { get; }
        public string DivisionName { get; }
        public DivisionTemplate Template { get; }

        public CreateDivisionProject(
            EconomicSubzoneHolding holding, Faction faction, string name, DivisionTemplate template)
            : base(holding, /* time= */ 10, template.GetMaterialCost())
        {
            Faction = faction;
            DivisionName = name;
            Template = template;
        }

        protected override void CancelImpl()
        {
            Holding.RemoveProject(this);
        }

        public override void Setup()
        {
            Holding.AddProject(this);
        }

        public override void Finish(World world)
        {
            Holding.RemoveProject(this);
            var division = new Division(Faction, Template);
            division.Add(Template.Composition);
            division.SetName(DivisionName);
            world.Formations.AddDivision(division);
            Holding.AddDivision(division);
        }
    }
}
