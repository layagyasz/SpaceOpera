using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Advancement
{
    class AdvancementSlot
    {
        private int _Progress;
        private IAdvancement _WorkingAdvancement;

        public void SetWorkingAdvancement(IAdvancement Advancement)
        {
            _WorkingAdvancement = Advancement;
        }

        public IMaterial GetResearchMaterial()
        {
            return _WorkingAdvancement?.Type.Research;
        }

        public void AddProgress(int Progress)
        {
            _Progress += Progress;
        }

        public int GetProgress()
        {
            return _Progress;
        }

        public int GetCost()
        {
            return _WorkingAdvancement.Cost;
        }

        public bool IsComplete()
        {
            return _Progress >= _WorkingAdvancement.Cost;
        }

        public void Finish()
        {
            _Progress -= _WorkingAdvancement.Cost;
            _WorkingAdvancement = null;
        }
    }
}