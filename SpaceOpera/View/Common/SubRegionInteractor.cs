using Cardamom;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;

namespace SpaceOpera.View.Common
{
    public class SubRegionInteractor : IInteractive, IInitializable
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }

        private readonly ICollider3 _hitbox;

        public SubRegionInteractor(IElementController controller, ICollider3 hitbox)
        {
            Controller = controller;
            _hitbox = hitbox;
        }

        public void Initialize()
        {
            Controller.Bind(this);
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return _hitbox.GetRayIntersection(ray);
        }
    }
}
