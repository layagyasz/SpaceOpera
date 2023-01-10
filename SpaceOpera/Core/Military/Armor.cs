using SpaceOpera.Core.Designs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Military
{
    class Armor
    {
        public float Thickness { get; }
        public float Protection { get; }
        public float Coverage { get; }

        public Armor(float Thickness, float Protection, float Coverage)
        {
            this.Thickness = Thickness;
            this.Protection = Protection;
            this.Coverage = Coverage;
        }

        public static Armor FromComponent(IComponent Component)
        {
            return new Armor(
                Component.GetAttribute(ComponentAttribute.ARMOR_THICKNESS),
                Component.GetAttribute(ComponentAttribute.ARMOR_PROTECTION), 
                Component.GetAttribute(ComponentAttribute.ARMOR_COVERAGE));
        }

        public static Armor Combine(IEnumerable<Armor> Armors)
        {
            return new Armor(
                Armors.Select(x => x.Thickness).DefaultIfEmpty(0).Sum(),
                Armors.Select(x => x.Protection).DefaultIfEmpty(0).Sum(), 
                Armors.Select(x => x.Coverage).DefaultIfEmpty(0).Sum());
        }
    }
}