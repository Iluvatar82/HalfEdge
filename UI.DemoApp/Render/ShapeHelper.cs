using ObjectTK.Buffers;
using ObjectTK.Shaders;
using ObjectTK.Tools.Cameras;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using UI.DemoApp.Models.Programs;
using UI.DemoApp.Models.Shapes;

namespace UI.DemoApp.Render
{
    public class ShapeHelper
    {
        private Program _program;
        private WireframeShape _shape;
        private VertexArray _vertexArray;
        private Vector3 _position;
        public WireframeShape Shape => _shape;
        public int TriangleCount => _shape.Indices.Length / 4;

        public ShapeHelper(WireframeShape shape, Vector3 position, Program program)
        {
            _program = program;
            _shape = shape;
            _position = position;

            _shape.UpdateBuffers();

            _vertexArray = new VertexArray();
            _vertexArray.Bind();
            if (_program is not SimpleProgram simpleProgram)
                return;

            _vertexArray.BindAttribute(simpleProgram.InPosition, _shape.VertexBuffer);
            _vertexArray.BindAttribute(simpleProgram.InColor, _shape.ColorBuffer);
            _vertexArray.BindElementBuffer(_shape.IndexBuffer);
        }

        public void Render(int width, int height, Camera camera)
        {
            if (_program is not SimpleProgram simpleProgram)
                return;

            simpleProgram.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(_position) *
                Matrix4.LookAt(camera.State.Position, Vector3.UnitZ * -8, camera.State.Up) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _vertexArray.Bind();
            _vertexArray.BindElementBuffer(_shape.IndexBuffer);
            _vertexArray.DrawElements(_shape.DefaultMode, _shape.IndexBuffer.ElementCount);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _vertexArray.BindAttribute(simpleProgram.InColor, _shape.WireframeColorBuffer);
            simpleProgram.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(_position + Vector3.UnitZ * 0.0001f) *
                Matrix4.LookAt(camera.State.Position, Vector3.UnitZ * -8, camera.State.Up) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _vertexArray.DrawElements(_shape.DefaultMode, _shape.IndexBuffer.ElementCount);
            _vertexArray.BindAttribute(simpleProgram.InColor, _shape.ColorBuffer);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}
