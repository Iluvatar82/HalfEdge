using Framework;
using Models.Base;

namespace Models.Tests.Base
{
    [TestFixture]
    public class MeshTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Cunstruct_Empty_Mesh()
        {
            var mesh = new Mesh();

            Assert.Multiple(() =>
            {
                Assert.That(mesh.Borders, Is.Empty);
                Assert.That(mesh.HalfEdges, Is.Empty);
                Assert.That(mesh.EdgeCount, Is.Zero);
                Assert.That(mesh.Indices, Is.Empty);
                Assert.That(mesh.Polygons, Is.Empty);
                Assert.That(mesh.PolygonCount, Is.Zero);
                Assert.That(mesh.Vertices, Is.Empty);
            });
        }

        [Test]
        public void Cunstruct_Mesh_With_Vertices_Indices()
        {
            var mesh = new Mesh(new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 1, 0), new Vertex(0, 1, 0) }, new List<List<int>> { new List<int> { 0, 1, 2 } });

            Assert.Multiple(() =>
            {
                Assert.That(mesh.Borders, Is.Empty);
                Assert.That(mesh.HalfEdges, Is.Empty);
                Assert.That(mesh.EdgeCount, Is.Zero);
                Assert.That(mesh.Indices, Has.Count.EqualTo(1));
                Assert.That(mesh.Polygons, Is.Empty);
                Assert.That(mesh.PolygonCount, Is.Zero);
                Assert.That(mesh.Vertices, Has.Count.EqualTo(3));
            });
        }

        [Test]
        public void GetVertexIndex_Found()
        {
            var mesh = new Mesh(new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 1, 0), new Vertex(0, 1, 0) }, new List<List<int>>());
            var vertexIndex = mesh.GetVertexIndex(new Vertex(1, 1, 0));

            Assert.That(vertexIndex, Is.EqualTo(1));
        }

        [Test]
        public void GetVertexIndex_Found_Within()
        {
            var mesh = new Mesh(new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 1, 0), new Vertex(0, 1, 0) }, new List<List<int>>());
            var vertexIndex = mesh.GetVertexIndex(new Vertex(1 + Constants.Epsilon * 0.5, 1, 0));

            Assert.That(vertexIndex, Is.EqualTo(1));
        }

        [Test]
        public void GetVertexIndex_NotFound()
        {
            var mesh = new Mesh(new List<Vertex> { new Vertex(0, 0, 0), new Vertex(1, 1, 0), new Vertex(0, 1, 0) }, new List<List<int>>());
            var vertexIndex = mesh.GetVertexIndex(new Vertex(1.1, 1, 0));

            Assert.That(vertexIndex, Is.EqualTo(-1));
        }
    }
}
