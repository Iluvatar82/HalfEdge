using HalfEdge;
using HalfEdge.MeshModifications;
using Models.Base;
using ObjectTK.Shaders;
using ObjectTK.Tools.Cameras;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UI.DemoApp.Models.Programs;
using UI.DemoApp.Models.Shapes;
using UI.DemoApp.Render;

namespace UI.DemoApp.Models
{
    public class ExampleScene
    {
        private Camera _camera;
        private CameraBehavior _cameraBehavior;
        private SimpleProgram _program;
        private List<ShapeHelper> _shapeHelpers;
        public int TriangleCount;

        public CameraBehavior CameraBehavior { get => _cameraBehavior; set => _cameraBehavior = value; }


        public ExampleScene()
        {
            TriangleCount = 0;
            var distance = 8f;
            var offset = 1f;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Multisample);

            //GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.DepthTest);

            _camera = new Camera();

            _cameraBehavior = new NewGimbalBehavior()
            {
                Origin = Vector3.UnitZ * -8
            };

            _camera.SetBehavior(_cameraBehavior);
            _shapeHelpers = new List<ShapeHelper>();
            _program = ProgramFactory.Create<SimpleProgram>();
            _program.Use();

            var vertices = new List<Vertex> { new Vertex(-1, -1, 0), new Vertex(1, -1, 0), new Vertex(0, 1, 0) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 } };
            _shapeHelpers.Add(new ShapeHelper(new MeshShape(vertices, indices, Color.LightGray, Color.DarkGreen), new Vector3(-offset, -offset, -distance), _program));

            var subdivisionModifier = new SubdivideMesh_Modifier()
            {
                Iterations = 7,
                SubdivisionType = HalfEdge.Enumerations.SubdivisionType.Loop
            };

            var triangleMesh = MeshFactory.CreateMesh(vertices, indices);
            subdivisionModifier.Modify(triangleMesh);
            var subdividedTriangleMesh = subdivisionModifier.OutputMesh;
            _shapeHelpers.Add(new ShapeHelper(new MeshShape(subdividedTriangleMesh.Vertices.ToList(), subdividedTriangleMesh.Indices.ToList(), Color.LightGray, Color.DarkGreen), new Vector3(offset, -offset, -distance + 0.0001f), _program));

            vertices = new List<Vertex> { new Vertex(-1, 0, 0), new Vertex(1, 0, 0), new Vertex(0, 2, 0), new Vertex(0, 1, 2) };
            indices = new List<List<int>> { new List<int> { 2, 1, 0 }, new List<int> { 0, 1, 3 }, new List<int> { 1, 2, 3 }, new List<int> { 2, 0, 3 } };

            _shapeHelpers.Add(new ShapeHelper(new MeshShape(vertices, indices), new Vector3(-offset, offset, -distance), _program));

            var shapeMesh = MeshFactory.CreateMesh(vertices, indices);
            subdivisionModifier.Modify(shapeMesh);
            var subdividedShapeMesh = subdivisionModifier.OutputMesh;
            _shapeHelpers.Add(new ShapeHelper(new MeshShape(subdividedShapeMesh.Vertices.ToList(), subdividedShapeMesh.Indices.ToList()), new Vector3(offset, offset, -distance + 0.0001f), _program));
            TriangleCount = _shapeHelpers.Sum(sh => sh.TriangleCount);
        }

        public void Render(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _program.Use();

            foreach (var shapeHelper in _shapeHelpers)
                shapeHelper.Render(width, height, _camera);
        }

        internal void HandleViewChange(Vector2 delta, float scale)
        {
            if (delta != Vector2.Zero)
                _cameraBehavior.MouseMove(_camera.State, delta);

            if (scale != 1f)
                _cameraBehavior.MouseWheelChanged(_camera.State, scale);
        }
    }
}
