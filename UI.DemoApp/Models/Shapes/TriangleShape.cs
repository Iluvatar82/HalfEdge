using ObjectTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;
using System.Linq;

namespace UI.DemoApp.Models.Shapes
{
    public class TriangleShape : WireframeShape
    {
        private readonly Color DefaultColor = Color.White;
        private readonly Color DefaultWireframeColor = Color.OrangeRed;

        public TriangleShape(Color? color = null, Color? wireframeColor = null)
        {
            DefaultMode = PrimitiveType.Triangles;

            Vertices = new Vector3[3];
            Indices = new uint[3];

            Vertices[0] = new Vector3(-1, -1, 0);
            Vertices[1] = new Vector3(1, -1, 0);
            Vertices[2] = new Vector3(0, 1, 0);

            Indices = new uint[] { 0, 1, 2 };

            Colors = Enumerable.Repeat(color ?? DefaultColor, 3).Select(_ => _.ToRgba32()).ToArray();
            WireframeColors = Enumerable.Repeat(wireframeColor ?? DefaultWireframeColor, 3).Select(_ => _.ToRgba32()).ToArray();
        }
    }
}
