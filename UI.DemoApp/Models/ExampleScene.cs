using HalfEdge;
using HalfEdge.MeshModifications;
using Models.Base;
using ObjectTK.Buffers;
using ObjectTK.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UI.DemoApp.Models.Programs;
using UI.DemoApp.Models.Shapes;

namespace UI.DemoApp.Models
{
    public class ExampleScene
    {
        private SimpleProgram _program;

        private MeshShape _triangle;
        private MeshShape _subdividedTriangle;
        private MeshShape _shape;
        private MeshShape _subdividedShape;

        private VertexArray _triangleVao;
        private VertexArray _subdividedTriangleVao;
        private VertexArray _shapeVao;
        private VertexArray _subdividedShapeVao;

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
            var subdividedTriangleMesh = subdivisionModifier.OutputMesh;
            _subdividedTriangle = new MeshShape(subdividedTriangleMesh.Vertices.ToList(), subdividedTriangleMesh.Indices.ToList());
            _subdividedTriangle.UpdateBuffers();

            _subdividedTriangleVao = new VertexArray();
            _subdividedTriangleVao.Bind();
            _subdividedTriangleVao.BindAttribute(_program.InPosition, _subdividedTriangle.VertexBuffer);
            _subdividedTriangleVao.BindAttribute(_program.InColor, _subdividedTriangle.ColorBuffer);
            _subdividedTriangleVao.BindElementBuffer(_subdividedTriangle.IndexBuffer);

            vertices = new List<Vertex> { new Vertex(-1, -1, 0), new Vertex(0, 0, 0), new Vertex(0, 1, 0), new Vertex(1, -1, 0), };
            indices = new List<List<int>> { new List<int> { 0, 1, 2 }, new List<int> { 1, 3, 2 } };

            _shape = new MeshShape(vertices, indices);
            _shape.UpdateBuffers();

            _shapeVao = new VertexArray();
            _shapeVao.Bind();
            _shapeVao.BindAttribute(_program.InPosition, _shape.VertexBuffer);
            _shapeVao.BindAttribute(_program.InColor, _shape.ColorBuffer);
            _shapeVao.BindElementBuffer(_shape.IndexBuffer);

            var shapeMesh = MeshFactory.CreateMesh(vertices, indices);
            subdivisionModifier.Modify(shapeMesh);
            var subdividedShapeMesh = subdivisionModifier.OutputMesh;
            _subdividedShape = new MeshShape(subdividedShapeMesh.Vertices.ToList(), subdividedShapeMesh.Indices.ToList());
            _subdividedShape.UpdateBuffers();

            _subdividedShapeVao = new VertexArray();
            _subdividedShapeVao.Bind();
            _subdividedShapeVao.BindAttribute(_program.InPosition, _subdividedShape.VertexBuffer);
            _subdividedShapeVao.BindAttribute(_program.InColor, _subdividedShape.ColorBuffer);
            _subdividedShapeVao.BindElementBuffer(_subdividedShape.IndexBuffer);
        }

        public void Render(int width, int height)
        {
            var distance = 8f;
            var offset = 1.5f;
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _program.Use();

            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(-offset, -offset, -distance) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _triangleVao.Bind();
            _triangleVao.BindElementBuffer(_triangle.IndexBuffer);
            _triangleVao.DrawElements(_triangle.DefaultMode, _triangle.IndexBuffer.ElementCount);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _triangleVao.BindAttribute(_program.InColor, _triangle.WireframeColorBuffer);
            
            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(-offset, -offset, -distance + 0.0001f) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _triangleVao.DrawElements(_triangle.DefaultMode, _triangle.IndexBuffer.ElementCount);
            _triangleVao.BindAttribute(_program.InColor, _triangle.ColorBuffer);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(offset, -offset, -distance) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _subdividedTriangleVao.Bind();
            _subdividedTriangleVao.BindElementBuffer(_subdividedTriangle.IndexBuffer);
            _subdividedTriangleVao.DrawElements(_subdividedTriangle.DefaultMode, _subdividedTriangle.IndexBuffer.ElementCount);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _subdividedTriangleVao.BindAttribute(_program.InColor, _subdividedTriangle.WireframeColorBuffer);
            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(offset, -offset, -distance + 0.0001f) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _subdividedTriangleVao.DrawElements(_subdividedTriangle.DefaultMode, _subdividedTriangle.IndexBuffer.ElementCount);
            _subdividedTriangleVao.BindAttribute(_program.InColor, _subdividedTriangle.ColorBuffer);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(-offset, offset, -distance) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _shapeVao.Bind();
            _shapeVao.BindElementBuffer(_shape.IndexBuffer);
            _shapeVao.DrawElements(_shape.DefaultMode, _shape.IndexBuffer.ElementCount);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _shapeVao.BindAttribute(_program.InColor, _shape.WireframeColorBuffer);
            
            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(-offset, offset, -distance + 0.0001f) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _shapeVao.DrawElements(_shape.DefaultMode, _shape.IndexBuffer.ElementCount);
            _shapeVao.BindAttribute(_program.InColor, _shape.ColorBuffer);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(offset, offset, -distance) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _subdividedShapeVao.Bind();
            _subdividedShapeVao.BindElementBuffer(_subdividedShape.IndexBuffer);
            _subdividedShapeVao.DrawElements(_subdividedShape.DefaultMode, _subdividedShape.IndexBuffer.ElementCount);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            _subdividedShapeVao.BindAttribute(_program.InColor, _subdividedShape.WireframeColorBuffer);
            
            _program.ModelViewProjectionMatrix.Set(
                Matrix4.CreateTranslation(offset, offset, -distance + 0.0001f) *
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, width / (float)height, 0.1f, 100));
            _subdividedShapeVao.DrawElements(_subdividedShape.DefaultMode, _subdividedShape.IndexBuffer.ElementCount);
            _subdividedShapeVao.BindAttribute(_program.InColor, _subdividedShape.ColorBuffer);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

        }
    }
}
