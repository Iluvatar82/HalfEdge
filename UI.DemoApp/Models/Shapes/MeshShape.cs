using Models.Base;
using ObjectTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UI.DemoApp.Data.Converter;

namespace UI.DemoApp.Models.Shapes
{
    internal class MeshShape : WireframeShape
    {
        private readonly Color DefaultColor = Color.White;
        private readonly Color DefaultWireframeColor = Color.OrangeRed;

        public MeshShape(List<Vertex> vertices, List<List<int>> indices, Color? color = null, Color? wireframeColor = null)
        {
            DefaultMode = PrimitiveType.Triangles;

            Vertices = vertices.Select(v => v.ToVector3()).ToArray();
            Indices = indices.SelectMany(t => t.Select(i => (uint)i)).ToArray();

            Colors = Enumerable.Repeat(color ?? DefaultColor, Vertices.Length).Select(_ => _.ToRgba32()).ToArray();
            WireframeColors = Enumerable.Repeat(wireframeColor ?? DefaultWireframeColor, Vertices.Length).Select(_ => _.ToRgba32()).ToArray();
        }
    }
}
