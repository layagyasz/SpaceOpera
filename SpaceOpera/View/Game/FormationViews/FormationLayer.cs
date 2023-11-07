using Cardamom.Graphics;
using SpaceOpera.Controller.Game.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Components;

namespace SpaceOpera.View.Game.FormationViews
{
    public class FormationLayer<T> : DynamicUiGroup where T : notnull
    {
        private readonly EventBuffer<IFormationDriver> _createEvents;
        private readonly EventBuffer<MovementEventArgs> _moveEvents;
        private readonly EventBuffer<IFormationDriver> _removeEvents;

        private readonly FormationManager _formationManager;
        private readonly IFormationLayerMapper<T> _mapper;
        private readonly Dictionary<object, FormationSubLayer<T>> _subLayers = new();

        private bool _dirty;

        private FormationLayer(FormationManager formationManager, IFormationLayerMapper<T> mapper)
            : base(new FormationLayerController<T>())
        {
            _createEvents = new(HandleCreate);
            _moveEvents = new(HandleMove);
            _removeEvents = new(HandleRemove);

            _formationManager = formationManager;
            _mapper = mapper;
        }

        public FormationLayer(
            FormationManager formationManager, IFormationLayerMapper<T> mapper, FormationSubLayer<T> singleSubLayer)
            : this(formationManager, mapper)
        {
            _subLayers.Add(singleSubLayer.Key, singleSubLayer);
            Add(singleSubLayer);
            Dirty();
        }

        public FormationLayer(
            FormationManager formationManager, 
            IFormationLayerMapper<T> mapper, 
            IEnumerable<FormationSubLayer<T>> subLayers)
            : this(formationManager, mapper)
        {
            foreach (var subLayer in subLayers)
            {
                _subLayers.Add(subLayer.Key, subLayer);
                Add(subLayer);
            }
            Dirty();
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
            _formationManager.Created -= _createEvents.QueueEvent;
            _formationManager.Moved -= _moveEvents.QueueEvent;
            _formationManager.Removed -= _removeEvents.QueueEvent;

            base.DisposeImpl();
        }

        public override void Initialize()
        {
            _formationManager.Created += _createEvents.QueueEvent;
            _formationManager.Moved += _moveEvents.QueueEvent;
            _formationManager.Removed += _removeEvents.QueueEvent;

            foreach (var formation in _formationManager.GetAtomicDrivers())
            {
                Add(formation);
            }

            base.Initialize();
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
            _createEvents.DispatchEvents();
            _moveEvents.DispatchEvents();
            _removeEvents.DispatchEvents();
            base.Update(delta);
        }

        private void Add(AtomicFormationDriver driver)
        {
            Add(driver, driver.AtomicFormation.Position, /* initialize= */ false);
        }

        private void HandleCreate(object? sender, IFormationDriver e)
        {
            if (e is AtomicFormationDriver driver)
            {
                Add(driver, driver.AtomicFormation.Position, /* initialize= */ true);
            }
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            var driver = (AtomicFormationDriver)sender!;
            Remove(driver, e.Origin);
            Add(driver, e.Destination, /* initialize= */ true);
        }

        private void HandleRemove(object? sender, IFormationDriver e)
        {
            if (e is AtomicFormationDriver driver)
            {
                Remove(driver);
            }
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

        private void Remove(AtomicFormationDriver driver)
        {
            Remove(driver, driver.AtomicFormation.Position);
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
