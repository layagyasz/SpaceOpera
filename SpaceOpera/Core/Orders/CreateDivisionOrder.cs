using SpaceOpera.Core.Economics;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Orders
{
    class CreateDivisionOrder : TimedOrder, IProject
    {
        public StellarBodyHolding Holding { get; }
        public DivisionTemplate Template { get; }

        public CreateDivisionOrder(StellarBodyHolding Holding, DivisionTemplate Template)
            : base(1)
        {
            this.Holding = Holding;
            this.Template = Template;
        }

        public override ValidationFailureReason Validate()
        {
            return ValidationFailureReason.NONE;
        }

        public override void Setup()
        {
            Holding.AddProject(this);
        }

        public override void Cleanup()
        {
            Holding.RemoveProject(this);
            // TODO: Reimplement
            // var division = new Division(Template.Name, Holding.Owner, Template, Template.Composition);
            // division.SetLocation(Holding.StellarBody);
        }
    }
}