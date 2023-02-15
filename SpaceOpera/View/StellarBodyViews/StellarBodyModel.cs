﻿using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;
using SpaceOpera.Core.Universe;

namespace SpaceOpera.View.StellarBodyViews
{
    public class StellarBodyModel : GraphicsResource, IRenderable
    {
        private static readonly int s_AtmospherePrecision = 10;

        private readonly StellarBody _stellarBody;
        private readonly float _scale;
        private Model<VertexLit3>? _surfaceModel;
        private VertexBuffer<VertexLit3>? _atmosphereModel;
        private readonly RenderShader _atmosphereShader;

        public StellarBodyModel(
            StellarBody stellarBody,
            float scale, 
            Model<VertexLit3> surfaceModel, 
            VertexBuffer<VertexLit3> atmosphereModel,
            RenderShader atmosphereShader)
        {
            _stellarBody = stellarBody;
            _scale = scale;
            _surfaceModel = surfaceModel;
            _atmosphereModel = atmosphereModel;
            _atmosphereShader = atmosphereShader;
        }

        public void Initialize()
        {
            _surfaceModel!.Initialize();
        }

        public void ResizeContext(Vector3 bounds)
        {
            _surfaceModel!.ResizeContext(bounds);
        }

        public void Draw(RenderTarget target, UiContext context)
        {
            _atmosphereShader.SetFloat("outer_radius", _scale * _stellarBody.Atmosphere.Radius);
            _atmosphereShader.SetFloat("inner_radius", _scale * _stellarBody.Radius);
            _atmosphereShader.SetFloat("atmosphere_density", _stellarBody.Atmosphere.Density);
            _atmosphereShader.SetFloat("atmosphere_density_falloff", _stellarBody.Atmosphere.Falloff);
            _atmosphereShader.SetInt32("atmosphere_precision", s_AtmospherePrecision);

            _surfaceModel!.Draw(target, context);
            target.Draw(
                _atmosphereModel!, 
                0,
                _atmosphereModel!.Length, 
                new RenderResources(BlendMode.Alpha, _atmosphereShader));
        }

        public void Update(long delta)
        {
            _surfaceModel!.Update(delta);
        }

        protected override void DisposeImpl()
        {
            _surfaceModel!.Buffer.Dispose();
            _surfaceModel = null;
            _atmosphereModel!.Dispose();
            _atmosphereModel = null;
        }
    }
}