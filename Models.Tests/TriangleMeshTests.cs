using Models.Base;

namespace Models.Tests
{
    [TestFixture]
    public class TriangleMeshTests
    {
        private List<Vertex> vertices;
        private List<List<int>> indices;
        private HalfEdge[] halfEdges;
        private List<Polygon> polygons;

        [SetUp]
        public void SetUp()
        {
            vertices = new List<Vertex> { new Vertex(1, 0, 0), new Vertex(3, 3, 0), new Vertex(2, 4, 0), new Vertex(0, 2, 0) };
            indices = new List<List<int>> { new List<int> { 0, 1, 2 }, new List<int> { 2, 3, 0 } };
            var sharedHalfEdge = new HalfEdge(vertices[2], vertices[0]);
            halfEdges = new HalfEdge[] {
                new HalfEdge(vertices[0], vertices[1]), new HalfEdge(vertices[1], vertices[2]), sharedHalfEdge,
                sharedHalfEdge.CreateOpposite(), new HalfEdge(vertices[2], vertices[3]), new HalfEdge(vertices[3], vertices[0])
            };
            polygons = new List<Polygon> { new Polygon(halfEdges[..3]), new Polygon(halfEdges[3..]) };
        }

        [Test]
        public void Constructor_Full()
        {
            var triangleMesh = new TriangleMesh(vertices, indices, halfEdges.ToList(), polygons);

            Assert.Multiple(() =>
            {
                Assert.That(triangleMesh.Borders.ToList(), Has.Count.EqualTo(1));
                Assert.That(triangleMesh.HalfEdges, Has.Count.EqualTo(6));
                Assert.That(triangleMesh.EdgeCount, Is.EqualTo(5));
                Assert.That(triangleMesh.Indices, Has.Count.EqualTo(2));
                Assert.That(triangleMesh.IsOpenMesh, Is.True);
                Assert.That(triangleMesh.PolygonCount, Is.EqualTo(2));
                Assert.That(triangleMesh.Polygons, Has.Count.EqualTo(2));
                Assert.That(triangleMesh.Vertices, Has.Count.EqualTo(4));
            });
        }

        [Test]
        public void AddPolygon_OK()
        {
            var triangleMesh = new TriangleMesh(vertices, indices, halfEdges.ToList(), polygons);
            triangleMesh.AddVertex(new Vertex(1, 0, 2));
            triangleMesh.AddIndices(new List<int> { 4, 0, 3 });
            var newHalfEdges = new List<HalfEdge> { new HalfEdge(vertices[4], vertices[0]), halfEdges.Last().CreateOpposite(), new HalfEdge(vertices[3], vertices[4]) };
            triangleMesh.AddHalfEdges(newHalfEdges);
            triangleMesh.AddPolygon(new Polygon(newHalfEdges));

            Assert.Multiple(() =>
            {
                Assert.That(triangleMesh.Borders.ToList(), Has.Count.EqualTo(1));
                Assert.That(triangleMesh.Borders.ElementAt(0), Has.Count.EqualTo(5));
                Assert.That(triangleMesh.HalfEdges, Has.Count.EqualTo(9));
                Assert.That(triangleMesh.EdgeCount, Is.EqualTo(7));
                Assert.That(triangleMesh.Indices, Has.Count.EqualTo(3));
                Assert.That(triangleMesh.IsOpenMesh, Is.True);
                Assert.That(triangleMesh.PolygonCount, Is.EqualTo(3));
                Assert.That(triangleMesh.Polygons, Has.Count.EqualTo(3));
                Assert.That(triangleMesh.Vertices, Has.Count.EqualTo(5));
            });
        }

        [Test]
        public void AddPolygon_TooMany_HalfEdges()
        {
            var triangleMesh = new TriangleMesh(vertices, indices, halfEdges.ToList(), polygons);
            triangleMesh.AddVertices(new List<Vertex> { new Vertex(1, 0, 2), new Vertex(0, 2, 2) });
            triangleMesh.AddIndices(new List<int> { 4, 0, 3, 5 });
            var newHalfEdges = new List<HalfEdge> { new HalfEdge(vertices[4], vertices[0]), halfEdges.Last().CreateOpposite(), new HalfEdge(vertices[3], vertices[5]), new HalfEdge(vertices[5], vertices[4]) };
            triangleMesh.AddHalfEdges(newHalfEdges);
            Assert.Throws<ArgumentOutOfRangeException>(() => { triangleMesh.AddPolygon(new Polygon(newHalfEdges)); });
        }
    }
}
