﻿using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using Cardamom.Ui.Elements;
using OpenTK.Mathematics;
using SpaceOpera.Core.Military;
using SpaceOpera.View.Icons;

namespace SpaceOpera.View.FormationViews
{
    public class FormationList : UiSerialContainer
    {
        private static readonly string s_FormationLayerTableClassName = "formation-layer-table";

        private readonly Vector3 _pin;
        private readonly float? _offset;
        private readonly UiElementFactory _uiElementFactory;
        private readonly IconFactory _iconFactory;

        private readonly Dictionary<object, FormationRow> _rows = new();

        private Vector4 _position;

        public FormationList(
            Vector3 position, float? offset, UiElementFactory uiElementFactory, IconFactory iconFactory)
            : base(
                  uiElementFactory.GetClass(s_FormationLayerTableClassName),
                  new TableController(0), 
                  Orientation.Vertical)
        {
            _pin = position;
            _offset = offset;
            _uiElementFactory = uiElementFactory;
            _iconFactory = iconFactory;
        }

        public void Add(IFormation formation)
        {
            var row = 
                FormationRow.Create(
                    formation, formation.Name, _uiElementFactory, _iconFactory.ForFaction(formation.Faction));
            row.Add(formation);
            _rows.Add(formation, row);
            Add(row);
        }

        public override void Draw(RenderTarget target, UiContext context)
        {
            if (_offset == null)
            {
                Position = _position.Xyz - 0.5f * Size;
            }
            else
            {
                Position = new(_position.X - 0.5f * Size.X, _position.Y + _offset.Value / _position.W, _position.Z);
            }
            base.Draw(target, context);
        }

        public IEnumerable<IFormation> GetFormations()
        {
            return _rows.Values.SelectMany(x => x.GetFormations());
        }

        public void Remove(IFormation formation)
        {
            if (_rows.TryGetValue(formation, out var row))
            {
                Remove(row);
                _rows.Remove(formation);
                row.Dispose();
            }
        }

        public void UpdateFromCamera(Matrix4 camera, UiContext context)
        {
            var transformed = new Vector4(_pin, 1) * camera;
            Visible = transformed.Z > 0;
            if (Visible)
            {
                var t = transformed / transformed.W;
                float d = -10 * t.Z;
                var window = context.NdcToWindow(t.Xy);
                _position = new(window.X, window.Y, d, transformed.W);
            }
        }
    }
}