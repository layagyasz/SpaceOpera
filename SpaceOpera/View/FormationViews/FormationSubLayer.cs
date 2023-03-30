using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Controller.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationSubLayer<T> : UiGroup where T : notnull
    {
        public object Key { get; }

        private readonly float? _offset;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly Dictionary<T, FormationList> _formationLists = new();

        private bool _dirty;

        public FormationSubLayer(
            object key,
            float? offset,
            UiElementFactory uiElementFactory, 
            IconFactory iconFactory)
            : base(new FormationSubLayerController<T>())
        {
            Key = key;
            _offset = offset;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            Dirty();
        }

        public void Dirty()
        {
            _dirty = true;
        }

        public IEnumerable<IFormation> GetFormations()
        {
            return _formationLists.Values.SelectMany(x => x.GetFormations());
        }

        public void UpdateFromCamera(Matrix4 camera, UiContext context)
        {
            foreach (var list in _formationLists.Values)
            {
                list.UpdateFromCamera(camera, context);
            }
            _elements.Sort((x, y) => ((FormationList)x).Position.Z.CompareTo(((FormationList)y).Position.Z));
            _dirty = false;
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

        public void Add(IFormation formation, T bucket, Vector3 pin)
        {
            if (!_formationLists.TryGetValue(bucket, out var list))
            {
                list = new FormationList(pin, _offset, _uiElementFactory, _iconFactory);
                _formationLists.Add(bucket, list);
                Add(list);
                Dirty();
            }
            list.Add(formation);
        }

        public void Remove(IFormation formation, T bucket)
        {
            if (_formationLists.TryGetValue(bucket, out var list))
            {
                list.Remove(formation);
                if (!list.Any())
                {
                    _formationLists.Remove(bucket);
                    Remove(list);
                    list.Dispose();
                }
            }
        }
    }
}
