using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Controller.FormationsViews;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Components;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationSubLayer<T> : DynamicUiGroup where T : notnull
    {
        public object Key { get; }

        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly Dictionary<T, FormationList> _formationLists = new();

        private bool _dirty;

        public FormationSubLayer(
            object key,
            UiElementFactory uiElementFactory, 
            IconFactory iconFactory)
            : base(new FormationSubLayerController<T>())
        {
            Key = key;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
            Dirty();
        }

        public void Dirty()
        {
            _dirty = true;
        }

        public IEnumerable<AtomicFormationDriver> GetDrivers()
        {
            return _formationLists.Values.SelectMany(x => x.GetDrivers());
        }

        public void UpdateFromCamera(Matrix4 transform)
        {
            foreach (var list in _formationLists.Values)
            {
                list.UpdateFromCamera(transform);
            }
            _elements.Sort((x, y) => ((FormationList)x).Position.Z.CompareTo(((FormationList)y).Position.Z));
            _dirty = false;
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
                foreach (var list in _formationLists.Values)
                {
                    list.UpdateFromCamera(transform);
                }
                _elements.Sort((x, y) => ((FormationList)x).Position.Z.CompareTo(((FormationList)y).Position.Z));
                _dirty = false;
            }
        }

        public void Add(AtomicFormationDriver driver, T bucket, Vector3 pin, float? offset, bool initialize)
        {
            if (!_formationLists.TryGetValue(bucket, out var list))
            {
                list = new FormationList(pin, offset, _uiElementFactory, _iconFactory);
                if (initialize)
                {
                    list.Initialize();
                }
                _formationLists.Add(bucket, list);
                Add(list);
                Dirty();
            }
            list.Add(driver, initialize);
        }

        public void Remove(AtomicFormationDriver driver, T bucket)
        {
            if (_formationLists.TryGetValue(bucket, out var list))
            {
                list.Remove(driver);
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
