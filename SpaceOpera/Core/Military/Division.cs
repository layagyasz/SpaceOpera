using SpaceOpera.Core.Politics;
using SpaceOpera.Core.Universe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class Division : BaseFormation
    {
        public DivisionTemplate Template { get; }
        public StellarBody StellarBodyLocation { get; private set; }

        public Division(string Name, Faction Faction, DivisionTemplate Template, MultiCount<Unit> Composition)
            : base(Faction)
        {
            SetName(Name);
            this.Template = Template;
        }
    }
}