using HalfEdge.MeshModifications;
using Models.Base;

namespace HalfEdge.Tests.MeshModifications
{
    [TestFixture]
    public class SubdivideMesh_ModifierTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void CreateOutputMesh_ValuesNotSet()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(1, 1, 0), new Vertex(1, 1, 1) };
            var indices = new List<List<int>> { new List<int> { 2, 1, 0 }, new List<int> { 0, 1, 3 }, new List<int> { 1, 2, 3 }, new List<int> { 2, 0, 3 } };
            var mesh = MeshFactory.CreateMesh(vertices, indices);

            var meshModifier = new SubdivideMesh_Modifier();
            Assert.Throws<ArgumentOutOfRangeException>(() => meshModifier.Modify(mesh));
        }

        [Test]
        public void CreateOutputMesh_Simple_Ok()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(2, 0, 0), new Vertex(1, 2, 0) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 } };
            var mesh = MeshFactory.CreateMesh(vertices, indices);


            var meshModifier = new SubdivideMesh_Modifier()
            {
                SubdivisionType = Enumerations.SubdivisionType.Loop,
                Iterations = 1
            };

            meshModifier.Modify(mesh);
            var triangleMesh = meshModifier.OutputMesh;

            Assert.Multiple(() =>
            {
                Assert.That(triangleMesh.Vertices, Has.Count.EqualTo(6));
                Assert.That(triangleMesh.Indices, Has.Count.EqualTo(4));
                Assert.That(triangleMesh.HalfEdges, Has.Count.EqualTo(12));
                Assert.That(triangleMesh.Polygons, Has.Count.EqualTo(4));
                Assert.That(triangleMesh.Polygons.All(p => p.HalfEdges.Count == 3), Is.True);
                Assert.That(triangleMesh.PolygonCount, Is.EqualTo(4));
                Assert.That(triangleMesh.IsOpenMesh, Is.True);
                Assert.That(triangleMesh.Borders.ToList(), Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void CreateOutputMesh_Ok()
        {
            var vertices = new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 0, 0), new Vertex(1, 1, 0), new Vertex(1, 1, 1) };
            var indices = new List<List<int>> { new List<int> { 2, 1, 0 }, new List<int> { 0, 1, 3 }, new List<int> { 1, 2, 3 }, new List<int> { 2, 0, 3 } };
            var mesh = MeshFactory.CreateMesh(vertices, indices);


            var meshModifier = new SubdivideMesh_Modifier()
            {
                SubdivisionType = Enumerations.SubdivisionType.Loop,
                Iterations = 1
            };

            meshModifier.Modify(mesh);
            var triangleMesh = meshModifier.OutputMesh;

            Assert.Multiple(() =>
            {
                Assert.That(triangleMesh.Vertices, Has.Count.EqualTo(10));
                Assert.That(triangleMesh.Indices, Has.Count.EqualTo(16));
                Assert.That(triangleMesh.HalfEdges, Has.Count.EqualTo(48));
                Assert.That(triangleMesh.HalfEdges.All(h => h.IsBorder), Is.False);
                Assert.That(triangleMesh.Polygons, Has.Count.EqualTo(16));
                Assert.That(triangleMesh.Polygons.All(p => p.IsBorder), Is.False);
                Assert.That(triangleMesh.Polygons.All(p => p.HalfEdges.Count == 3), Is.True);
                Assert.That(triangleMesh.PolygonCount, Is.EqualTo(16));
                Assert.That(triangleMesh.IsOpenMesh, Is.False);
                Assert.That(triangleMesh.Borders, Is.Empty);
            });
        }
    }
}
