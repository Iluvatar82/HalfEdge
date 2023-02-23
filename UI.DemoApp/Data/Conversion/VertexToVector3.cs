using Models.Base;
using OpenTK.Mathematics;

namespace UI.DemoApp.Data.Conversion
{
    public static class VertexToVector3
    {
        public static Vector3 ToVector3(this Vertex vertex) => new ((float)vertex.X, (float)vertex.Y, (float)vertex.Z);
    }
}
