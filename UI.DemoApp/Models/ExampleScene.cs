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
        private NewGimbalBehavior _cameraBehavior;
        private SimpleProgram _program;
        private List<ShapeHelper> _shapeHelpers;
        public int PrimitiveCount { get; private set; }

        public NewGimbalBehavior CameraBehavior { get => _cameraBehavior; set => _cameraBehavior = value; }


        public ExampleScene()
        {
            PrimitiveCount = 0;
            var distance = 8f;
            var xOffset = 3f;
            var yOffset = 1.5f;

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

            var vertices = new List<Vertex> {
                new Vertex(-1, -1, -1), new Vertex( 1, -1, -1), new Vertex( 1,  1, -1), new Vertex(-1,  1, -1),
                new Vertex(-1, -1,  1), new Vertex( 1, -1,  1), new Vertex( 1,  1,  1), new Vertex(-1,  1,  1)
            };
            var indices = new List<List<int>> {
                new List<int> { 3, 2, 1, 0 },
                new List<int> { 0, 1, 5, 4 }, new List<int> { 1, 2, 6, 5 }, new List<int> { 2, 3, 7, 6 }, new List<int> { 3, 0, 4, 7 },
                new List<int> { 4, 5, 6, 7 }
            };
            /*var vertices = new List<Vertex> {
                new Vertex(-1, -1, 0), new Vertex( 1, -1, 0), new Vertex( 1,  1, 0), new Vertex(-1,  1, 0),
            };
            var indices = new List<List<int>> {
                new List<int> { 0, 1, 2, 3 },
            };*/

            var subdivisionModifier = new MeshSubdivider()
            {
                Iterations = 6,
                SubdivisionType = HalfEdge.Enumerations.SubdivisionType.Loop
            };

            var baseMesh = MeshFactory.CreateMesh(vertices, indices);
            var triangulator = new MeshTriangulator();
            triangulator.Modify(baseMesh);
            var triangulatedMesh = triangulator.OutputMesh;
            _shapeHelpers.Add(new ShapeHelper(new MeshShape(triangulatedMesh.Vertices.ToList(), triangulatedMesh.Indices.ToList(), Color.LightGray, Color.DarkGreen), new Vector3(-xOffset, yOffset, -distance), _program));
            
            subdivisionModifier.Modify(triangulatedMesh);
            var subdividedBaseMesh = subdivisionModifier.OutputMesh;
            _shapeHelpers.Add(new ShapeHelper(new MeshShape(subdividedBaseMesh.Vertices.ToList(), subdividedBaseMesh.Indices.ToList(), Color.LightGray, Color.DarkGreen), new Vector3(-xOffset, -yOffset, -distance), _program));

            _shapeHelpers.Add(new ShapeHelper(new MeshShape(baseMesh.Vertices.ToList(), baseMesh.Indices.ToList(), Color.LightGray, Color.DarkRed, PrimitiveType.Quads), new Vector3(0, yOffset, -distance), _program));

            subdivisionModifier.SubdivisionType = HalfEdge.Enumerations.SubdivisionType.CatmullClark;
            subdivisionModifier.Modify(baseMesh);
            var subdividedShapeMesh = subdivisionModifier.OutputMesh;
            _shapeHelpers.Add(new ShapeHelper(new MeshShape(subdividedShapeMesh.Vertices.ToList(), subdividedShapeMesh.Indices.ToList(), Color.LightGray, Color.DarkRed, PrimitiveType.Quads), new Vector3(0, -yOffset, -distance), _program));


            _shapeHelpers.Add(new ShapeHelper(new MeshShape(triangulatedMesh.Vertices.ToList(), triangulatedMesh.Indices.ToList(), Color.LightGray, Color.DarkRed), new Vector3(xOffset, yOffset, -distance), _program));
            
            subdivisionModifier.SubdivisionType = HalfEdge.Enumerations.SubdivisionType.ModifiedButterfly;
            subdivisionModifier.Modify(triangulatedMesh);
            var otherSubdividedShapeMesh = subdivisionModifier.OutputMesh;
            _shapeHelpers.Add(new ShapeHelper(new MeshShape(otherSubdividedShapeMesh.Vertices.ToList(), otherSubdividedShapeMesh.Indices.ToList(), Color.LightGray, Color.DarkRed), new Vector3(xOffset, -yOffset, -distance), _program));
            
            PrimitiveCount = _shapeHelpers.Sum(sh => sh.PrimitiveCount);
        }

        public void Render(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _program.Use();

            foreach (var shapeHelper in _shapeHelpers)
                shapeHelper.Render(width, height, _camera, _cameraBehavior.Origin);
        }

        internal void HandleViewChange(Vector2 delta, float scale, bool pan = false)
        {
            if (delta != Vector2.Zero && !pan)
                _cameraBehavior.MouseMove(_camera.State, delta);
            else if (delta != Vector2.Zero && pan)
                _cameraBehavior.MousePan(_camera.State, delta);
            
            if (scale != 1f)
                _cameraBehavior.MouseWheelChanged(_camera.State, scale);
        }
    }
}
