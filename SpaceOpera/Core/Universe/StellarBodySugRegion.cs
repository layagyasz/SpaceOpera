using Cardamom.Planar;
using Cardamom.Spatial;
using SFML.Window;
using SpaceOpera.Core.Military;
using SpaceOpera.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceOpera.Core.Universe
{
    class StellarBodySubRegion : INavigable
    {
        public EventHandler<ElementEventArgs<Division>> OnDivisionAdded { get; set; }

        public int Id { get; }
        public string Name { get { return ParentRegion.Name; } }
        public NavigableNodeType NavigableNodeType => NavigableNodeType.Ground;
        public Vector4f Center { get; }
        public CSpherical SphericalCenter { get; }
        public Biome Biome { get; }
        public Vector4f[] Bounds { get; private set; }

        public List<StellarBodySubRegion> Neighbors { get; private set; }
        public StellarBodyRegion ParentRegion { get; private set; }

        private List<Division> _Divisions = new List<Division>();

        public StellarBodySubRegion(int Id, Vector4f Center, CSpherical SphericalCenter, Biome Biome)
        {
            this.Id = Id;
            this.Center = Center;
            this.SphericalCenter = SphericalCenter;
            this.Biome = Biome;
        }

        public void SetParentRegion(StellarBodyRegion Region)
        {
            this.ParentRegion = Region;
        }

        public void SetNeighbors(IEnumerable<StellarBodySubRegion> Neighbors)
        {
            this.Neighbors = Neighbors.ToList();

            var comparer = new ClockwiseSurface3dComparer(SphericalCenter);
            this.Neighbors.Sort((x, y) => comparer.Compare(x.Center, y.Center));

            Bounds = new Vector4f[this.Neighbors.Count];
            for (int i = 0; i < this.Neighbors.Count; ++i)
            {
                Bounds[i] = 
                    SphericalCenter.Radius 
                    * ((Center + this.Neighbors[i].Center 
                    + this.Neighbors[(i + 1) % this.Neighbors.Count].Center) / 3).Normalize();
            }
        }

        public void AddDivision(Division Division)
        {
            _Divisions.Add(Division);
            OnDivisionAdded?.Invoke(this, new ElementEventArgs<Division>(Division));
        }

        public void RemoveDivision(Division Division)
        {
            _Divisions.Remove(Division);
        }

        public override string ToString()
        {
            return string.Format("[StellarBodySubRegion: Id={0}]", Id);
        }
    }
}