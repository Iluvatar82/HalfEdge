using HalfEdge;
using HalfEdge.MeshModifications;
using Models.Base;
using ObjectTK.Buffers;
using ObjectTK.Shaders;
using ObjectTK.Tools.Shapes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using UI.DemoApp.Models.Programs;
using UI.DemoApp.Models.Shapes;

namespace UI.DemoApp.Models
{
    public class ExampleScene
    {
        private SimpleProgram _program;

        private MeshShape _triangle;
        private MeshShape _subdividedTriangle;

        private VertexArray _triangleVao;
        private VertexArray _subdividedTriangleVao;

        public ExampleScene()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Multisample);

            //GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.DepthTest);


            _program = ProgramFactory.Create<SimpleProgram>();
            _program.Use();

            var vertices = new List<Vertex> { new Vertex(-1, -1, 0), new Vertex(1, -1, 0), new Vertex(0, 1, 0) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 } };

            _triangle = new MeshShape(vertices, indices);
            _triangle.UpdateBuffers();

            _triangleVao = new VertexArray();
            _triangleVao.Bind();
            _triangleVao.BindAttribute(_program.InPosition, _triangle.VertexBuffer);
            _triangleVao.BindAttribute(_program.InColor, _triangle.ColorBuffer);
            _triangleVao.BindElementBuffer(_triangle.IndexBuffer);


            var subdivisionModifier = new SubdivideMesh_Modifier()
            {
                Iterations = 5,
                SubdivisionType = HalfEdge.Enumerations.SubdivisionType.Loop
            };

            var triangleMesh = MeshFactory.CreateMesh(vertices, indices);
            subdivisionModifier.Modify(triangleMesh);
            var subdividedMesh = subdivisionModifier.OutputMesh;
            _subdividedTriangle = new MeshShape(subdividedMesh.Vertices.ToList(), subdividedMesh.Indices.ToList());
            _subdividedTriangle.UpdateBuffers();

            _subdividedTriangleVao = new VertexArray();
            _subdividedTriangleVao.Bind();
            _subdividedTriangleVao.BindAttribute(_program.InPosition, _subdividedTriangle.VertexBuffer);
            _subdividedTriangleVao.BindAttribute(_program.InColor, _subdividedTriangle.ColorBuffer);
            _subdividedTriangleVao.BindElementBuffer(_subdividedTriangle.IndexBuffer);
        }

        public void Render(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _program.Use();
            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(-1.5f, 0, -5) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));

            _triangleVao.Bind();
            _triangleVao.BindElementBuffer(_triangle.IndexBuffer);
            _triangleVao.DrawElements(_triangle.DefaultMode, _triangle.IndexBuffer.ElementCount);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _triangleVao.BindAttribute(_program.InColor, _triangle.WireframeColorBuffer);
            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(-1.5f, 0, -5 + 0.0001f) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _triangleVao.DrawElements(_triangle.DefaultMode, _triangle.IndexBuffer.ElementCount);
            _triangleVao.BindAttribute(_program.InColor, _triangle.ColorBuffer);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(1.5f, 0, -5) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));

            _subdividedTriangleVao.Bind();
            _subdividedTriangleVao.BindElementBuffer(_subdividedTriangle.IndexBuffer);
            _subdividedTriangleVao.DrawElements(_subdividedTriangle.DefaultMode, _subdividedTriangle.IndexBuffer.ElementCount);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _subdividedTriangleVao.BindAttribute(_program.InColor, _subdividedTriangle.WireframeColorBuffer);
            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(1.5f, 0, -5 + 0.0001f) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _subdividedTriangleVao.DrawElements(_triangle.DefaultMode, _subdividedTriangle.IndexBuffer.ElementCount);
            _subdividedTriangleVao.BindAttribute(_program.InColor, _subdividedTriangle.ColorBuffer);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}
