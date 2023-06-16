using Cardamom.Graphics;
using SpaceOpera.Controller.Game.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Game.FormationViews
{
    public class FormationLayer<T> : DynamicUiGroup where T : notnull
    {
        private readonly EventBuffer<MovementEventArgs> _events;
        private readonly IFormationLayerMapper<T> _mapper;
        private readonly Dictionary<object, FormationSubLayer<T>> _subLayers = new();

        private bool _dirty;

        public FormationLayer(IFormationLayerMapper<T> mapper, FormationSubLayer<T> singleSubLayer)
            : base(new FormationLayerController<T>())
        {
            _events = new(HandleMove);
            _mapper = mapper;
            _subLayers.Add(singleSubLayer.Key, singleSubLayer);
            Add(singleSubLayer);
            Dirty();
        }

        public FormationLayer(IFormationLayerMapper<T> mapper, IEnumerable<FormationSubLayer<T>> subLayers)
            : base(new FormationLayerController<T>())
        {
            _events = new(HandleMove);
            _mapper = mapper;
            foreach (var subLayer in subLayers)
            {
                _subLayers.Add(subLayer.Key, subLayer);
                Add(subLayer);
            }
            Dirty();
        }

        public void Add(AtomicFormationDriver driver)
        {
            driver.Moved += _events.QueueEvent;
            Add(driver, driver.AtomicFormation.Position, /* initialize= */ false);

        }

        public void Dirty()
        {
            _dirty = true;
            foreach (var subLayer in _subLayers.Values)
            {
                subLayer.Dirty();
            }
        }

        protected override void DisposeImpl()
        {
            foreach (var driver in _subLayers.Values.SelectMany(x => x.GetDrivers()))
            {
                driver.Moved -= _events.QueueEvent;
            }
            base.DisposeImpl();
        }


        public void Remove(AtomicFormationDriver driver)
        {
            driver.Moved -= _events.QueueEvent;
            Remove(driver, driver.AtomicFormation.Position);
        }

        public void UpdateFromCamera(IRenderTarget target)
        {
            if (_dirty)
            {
                var sceneProjection = target.GetProjection();
                target.PopProjectionMatrix();
                var uiProjection = target.GetProjection().Matrix;
                uiProjection.Invert();
                var transform = 
                    target.GetModelMatrix() * target.GetViewMatrix() * sceneProjection.Matrix * uiProjection;
                target.PushProjection(sceneProjection);
                foreach (var subLayer in _subLayers.Values)
                {
                    subLayer.UpdateFromCamera(transform);
                }
                _dirty = false;
            }
        }

        public override void Update(long delta)
        {
            _events.DispatchEvents();
            base.Update(delta);
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            var driver = (AtomicFormationDriver)sender!;
            Remove(driver, e.Origin);
            Add(driver, e.Destination, /* initialize= */ true);
        }

        private void Add(AtomicFormationDriver driver, INavigable? location, bool initialize)
        {
            if (location == null)
            {
                return;
            }
            (var layer, var bucket) = _mapper.MapToBucket(location);
            if (layer != null && _subLayers.TryGetValue(layer, out var subLayer))
            {
                subLayer.Add(driver, bucket, _mapper.MapToPin(bucket), _mapper.GetOffset(bucket), initialize);
                Dirty();
            }
        }

        private void Remove(AtomicFormationDriver driver, INavigable? location)
        {
            if (location == null)
            {
                return;
            }
            (var layer, var bucket) = _mapper.MapToBucket(location);
            if (layer != null && _subLayers.TryGetValue(layer, out var list))
            {
                list.Remove(driver, bucket);
            }
        }
    }
}
