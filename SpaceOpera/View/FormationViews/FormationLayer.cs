using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using SpaceOpera.Controller.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.FormationViews
{
    public class FormationLayer<T> : UiGroup where T : notnull
    {
        private readonly IFormationLayerMapper<T> _mapper;
        private readonly Dictionary<object, FormationSubLayer<T>> _subLayers = new();

        private bool _dirty;

        public FormationLayer(IFormationLayerMapper<T> mapper, FormationSubLayer<T> singleSubLayer)
            : base(new FormationLayerController<T>())
        {
            _mapper = mapper;
            _subLayers.Add(singleSubLayer.Key, singleSubLayer);
            Add(singleSubLayer);
            Dirty();
        }

        public FormationLayer(IFormationLayerMapper<T> mapper, IEnumerable<FormationSubLayer<T>> subLayers)
            : base(new FormationLayerController<T>())
        {
            _mapper = mapper;
            foreach (var subLayer in subLayers)
            {
                _subLayers.Add(subLayer.Key, subLayer);
                Add(subLayer);
            }
            Dirty();
        }

        public void Add(IFormation formation)
        {
            formation.Moved += HandleMove;
            Add(formation, formation.Position);

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
            foreach (var formation in _subLayers.Values.SelectMany(x => x.GetFormations()))
            {
                formation.Moved -= HandleMove;
            }
            base.DisposeImpl();
        }


        public void Remove(IFormation formation)
        {
            formation.Moved -= HandleMove;
            Remove(formation, formation.Position);
        }

        public void UpdateFromCamera(RenderTarget target, UiContext context)
        {
            if (_dirty)
            {
                var camera = target.GetModelMatrix() * target.GetViewMatrix() * target.GetProjection().Matrix;
                foreach (var subLayer in _subLayers.Values)
                {
                    subLayer.UpdateFromCamera(camera, context);
                }
                _dirty = false;
            }
        }

        private void HandleMove(object? sender, MovementEventArgs e)
        {
            IFormation formation = (IFormation)sender!;
            Remove(formation, e.Origin);
            Add(formation, e.Destination);
        }

        private void Add(IFormation formation, INavigable? location)
        {
            if (location == null)
            {
                return;
            }
            (var layer, var bucket) = _mapper.MapToBucket(location);
            if (layer != null && _subLayers.TryGetValue(layer, out var subLayer))
            {
                subLayer.Add(formation, bucket, _mapper.MapToPin(bucket), _mapper.GetOffset(bucket));
                Dirty();
            }
        }

        private void Remove(IFormation formation, INavigable? location)
        {
            if (location == null)
            {
                return;
            }
            (var layer, var bucket) = _mapper.MapToBucket(location);
            if (layer != null && _subLayers.TryGetValue(layer, out var list))
            {
                list.Remove(formation, bucket);
            }
        }
    }
}
