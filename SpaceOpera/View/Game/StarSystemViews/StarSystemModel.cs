﻿using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.View.Game.Common;
using SpaceOpera.View.Game.StarViews;

namespace SpaceOpera.View.Game.StarSystemViews
{
    public class StarSystemModel : GraphicsResource, IRenderable
    {
        private StarBuffer? _star;
        private TransitBuffer? _guidelines;

        public StarSystemModel(StarBuffer star, TransitBuffer guidelines) 
        {
            _star = star;
            _guidelines = guidelines;
        }

        public void Dirty()
        {
            _star!.Dirty();
        }

        protected override void DisposeImpl()
        {
            _star!.Dispose();
            _star = null;
            _guidelines!.Dispose();
            _guidelines = null;
        }

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _guidelines!.Draw(target, context);
            _star!.Draw(target, context);
        }

        public Color4 GetLightColor()
        {
            return _star!.Get(0).Color;
        }

        public void Initialize()
        {
            _star!.Initialize();
            _guidelines!.Initialize();
        }

        public void ResizeContext(Vector3 bounds) { }

        public void Update(long delta)
        {
            _star!.Update(delta);
            _guidelines!.Update(delta);
        }
    }
}
