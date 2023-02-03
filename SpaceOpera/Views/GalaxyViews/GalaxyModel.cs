﻿using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Views.StarViews;

namespace SpaceOpera.Views.GalaxyViews
{
    public class GalaxyModel : GraphicsResource, IRenderable
    {
        private StarBuffer? _stars;

        public GalaxyModel(StarBuffer stars)
        {
            _stars = stars;
        }

        public void Dirty()
        {
            _stars!.Dirty();
        }

        public void Initialize()
        {
            _stars!.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {
            _stars!.ResizeContext(bounds);
        }
        

        public void Draw(RenderTarget target, UiContext context)
        {
            _stars!.Draw(target, context);
        }

        public void Update(long delta)
        {
            _stars!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            _stars?.Dispose();
            _stars = null;
        }
    }
}
