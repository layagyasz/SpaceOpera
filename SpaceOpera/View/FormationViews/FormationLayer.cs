using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationLayer<T> : UiGroup where T : notnull
    {
        private readonly Func<INavigable, T> _groupFn;
        private readonly Func<T, Vector3> _positionFn;
        private readonly float? _offset;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly Dictionary<T, FormationList> _formationLists = new();

        private bool _dirty;

        public FormationLayer(
            Func<INavigable, T> groupFn, 
            Func<T, Vector3> positionFn,
            float? offset,
            UiElementFactory uiElementFactory, 
            IconFactory iconFactory)
            : base(new FormationLayerController<T>())
        {
            _groupFn = groupFn;
            _positionFn = positionFn;
            _offset = offset;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
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
        }

        protected override void DisposeImpl()
        {
            foreach (var list in _formationLists.Values)
            {
                foreach (var formation in list.GetFormations())
                {
                    formation.Moved -= HandleMove;
                }
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
                foreach (var list in _formationLists.Values)
                {
                    list.UpdateFromCamera(camera, context);
                }
                _elements.Sort((x, y) => ((FormationList)x).Position.Z.CompareTo(((FormationList)y).Position.Z));
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
            var group = _groupFn(location);
            if (!_formationLists.TryGetValue(group, out var list))
            {
                list = new FormationList(_positionFn(group), _offset, _uiElementFactory, _iconFactory);
                _formationLists.Add(group, list);
                Add(list);
            }
            list.Add(formation);
        }

        private void Remove(IFormation formation, INavigable? location)
        {
            if (location == null)
            {
                return;
            }
            var group = _groupFn(location);
            if (_formationLists.TryGetValue(group, out var list))
            {
                list.Remove(formation);
                if (!list.Any())
                {
                    _formationLists.Remove(group);
                    Remove(list);
                    list.Dispose();
                }
            }
        }
    }
}
