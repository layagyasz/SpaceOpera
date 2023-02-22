﻿using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;
using SpaceOpera.View.Common;
using SpaceOpera.View.Scenes.Highlights;
using SpaceOpera.View.StarSystemViews;
using SpaceOpera.View.StarViews;

namespace SpaceOpera.View.Scenes
{
    public class StarSystemScene : GraphicsResource, IGameScene
    {
        public IElementController Controller { get; }
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; set; }

        private StarBuffer? _starBuffer;
        private StarSubSystemRig[]? _subSystems;
        private TransitBuffer? _guidelines;
        private readonly SubRegionInteractor[] _interactors;
        private HighlightLayer<INavigable>? _highlightLayer;
        private readonly Skybox _skybox;

        public StarSystemScene(
            IElementController controller,
            ICamera camera,
            StarBuffer starBuffer,
            StarSubSystemRig[] subSystems,
            TransitBuffer guidelines,
            SubRegionInteractor[] interactors,
            HighlightLayer<INavigable> highlightLayer,
            Skybox skybox)
        {
            Controller = controller;
            Camera = camera;
            _starBuffer = starBuffer;
            _subSystems = subSystems;
            foreach (var subSystem in subSystems)
            {
                subSystem.Parent = this;
            }
            _guidelines = guidelines;
            _interactors = interactors;
            _highlightLayer = highlightLayer;
            _skybox = skybox;
        }

        protected override void DisposeImpl()
        {
            _starBuffer!.Dispose();
            _starBuffer = null;
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Dispose();
            }
            _subSystems = null;
            _guidelines!.Dispose();
            _guidelines = null;
            _highlightLayer!.Dispose();
            _highlightLayer = null;
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            foreach (var interactor in _interactors)
            {
                context.Register(interactor);
            }

            _skybox.Draw(target, context);
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Draw(target, context);
            }
            _highlightLayer!.Draw(target, context);
            _guidelines!.Draw(target, context);
            _starBuffer!.Draw(target, context);

            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            _starBuffer!.Initialize();
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Initialize();
            }
            Controller.Bind(this);
        }

        public void ResizeContext(Vector3 bounds)
        {
            Camera.SetAspectRatio(bounds.X / bounds.Y);
        }

        public void SetHighlight(HighlightLayerName layer, ICompositeHighlight highlight)
        {
            _highlightLayer!.SetLayer(layer, highlight);
            foreach (var subSystem in _subSystems!)
            {
                subSystem.SetHighlight(layer, highlight);
            }
        }

        public void Update(long delta)
        {
            foreach (var subSystem in _subSystems!)
            {
                subSystem.Update(delta);
            }
        }
    }
}
