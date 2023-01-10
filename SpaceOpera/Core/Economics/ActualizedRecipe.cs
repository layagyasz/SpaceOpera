using SpaceOpera.Core.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Economics
{
    class ActualizedRecipe
    {
        public int SubKey { get; }
        public Recipe BaseRecipe { get; }
        public MultiQuantity<IMaterial> Transformation { get; }

        public ActualizedRecipe(int SubKey, Recipe BaseRecipe, MultiQuantity<IMaterial> Transformation)
        {
            this.SubKey = SubKey;
            this.BaseRecipe = BaseRecipe;
            this.Transformation = Transformation;
        }

        public override int GetHashCode()
        {
            if (BaseRecipe.BoundResourceNode == null)
            {
                return BaseRecipe.GetHashCode();
            }
            return SubKey ^ BaseRecipe.GetHashCode();
        }
    }
}