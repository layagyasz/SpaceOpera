﻿using Cardamom.Graphics;
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

        public IEnumerable<IFormationDriver> GetDrivers()
        {
            return _formationLists.Values.SelectMany(x => x.GetDrivers());
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

        public void Add(IFormationDriver driver, T bucket, Vector3 pin, float? offset)
        {
            if (!_formationLists.TryGetValue(bucket, out var list))
            {
                list = new FormationList(pin, offset, _uiElementFactory, _iconFactory);
                list.Initialize();
                _formationLists.Add(bucket, list);
                Add(list);
                Dirty();
            }
            list.Add(driver);
        }

        public void Remove(IFormationDriver driver, T bucket)
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
