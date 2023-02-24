using Framework.Extensions;
using Models.Base;

namespace Models.Tests
{
    [TestFixture]
    public class QuadMeshTests
    {
        private List<Vertex> vertices;
        private List<List<int>> indices;
        private HalfEdge[] halfEdges;
        private List<Polygon> polygons;

        [SetUp]
        public void SetUp()
        {
            vertices = new List<Vertex> { new Vertex(1, 0, 0), new Vertex(3, 3, 0), new Vertex(2, 4, 0), new Vertex(0, 2, 0) };
            indices = new List<List<int>> { new List<int> { 0, 1, 2, 3 } };
            halfEdges = new HalfEdge[] {
                new HalfEdge(vertices[0], vertices[1]), new HalfEdge(vertices[1], vertices[2]), new HalfEdge(vertices[2], vertices[3]), new HalfEdge(vertices[3], vertices[0])
            };
            polygons = new List<Polygon> { new Polygon(halfEdges) };
        }

        [Test]
        public void Constructor_Full()
        {
            var quadMesh = new QuadMesh(vertices, indices, halfEdges.ToList(), polygons);
            halfEdges.ForEach(h => quadMesh.BorderHalfEdgeDictionary.Add((h.Start, h.End), h));

            Assert.Multiple(() =>
            {
                Assert.That(quadMesh.Borders.ToList(), Has.Count.EqualTo(1));
                Assert.That(quadMesh.HalfEdges, Has.Count.EqualTo(4));
                Assert.That(quadMesh.EdgeCount, Is.EqualTo(4));
                Assert.That(quadMesh.Edges.ToList(), Has.Count.EqualTo(4));
                Assert.That(quadMesh.Indices, Has.Count.EqualTo(1));
                Assert.That(quadMesh.IsOpenMesh, Is.True);
                Assert.That(quadMesh.PolygonCount, Is.EqualTo(1));
                Assert.That(quadMesh.Polygons, Has.Count.EqualTo(1));
                Assert.That(quadMesh.Vertices, Has.Count.EqualTo(4));
            });
        }

        [Test]
        public void AddPolygon_OK()
        {
            var quadMesh = new QuadMesh(vertices, indices, halfEdges.ToList(), polygons);
            halfEdges.ForEach(h => quadMesh.BorderHalfEdgeDictionary.Add((h.Start, h.End), h));
            quadMesh.AddVertices(new List<Vertex> { new Vertex(1, 0, 2), new Vertex(3, 3, 2) });
            quadMesh.AddIndices(new List<int> { 4, 5, 1, 0 });

            var newHalfEdges = new List<HalfEdge> { new HalfEdge(vertices[4], vertices[5]), new HalfEdge(vertices[5], vertices[1]), halfEdges.First().CreateOpposite(), new HalfEdge(vertices[0], vertices[4]) };
            halfEdges.Where(h => h.Opposite is not null).ForEach(h => quadMesh.BorderHalfEdgeDictionary.Remove((h.Start, h.End)));
            newHalfEdges.Where(h => h.Opposite is null).ForEach(h => quadMesh.BorderHalfEdgeDictionary.Add((h.Start, h.End), h));
            quadMesh.AddHalfEdges(newHalfEdges);
            quadMesh.AddPolygon(new Polygon(newHalfEdges));

            Assert.Multiple(() =>
            {
                Assert.That(quadMesh.Borders.ToList(), Has.Count.EqualTo(1));
                Assert.That(quadMesh.Borders.ElementAt(0), Has.Count.EqualTo(6));
                Assert.That(quadMesh.HalfEdges, Has.Count.EqualTo(8));
                Assert.That(quadMesh.EdgeCount, Is.EqualTo(7));
                Assert.That(quadMesh.Edges.ToList(), Has.Count.EqualTo(7));
                Assert.That(quadMesh.Indices, Has.Count.EqualTo(2));
                Assert.That(quadMesh.IsOpenMesh, Is.True);
                Assert.That(quadMesh.PolygonCount, Is.EqualTo(2));
                Assert.That(quadMesh.Polygons, Has.Count.EqualTo(2));
                Assert.That(quadMesh.Vertices, Has.Count.EqualTo(6));
            });
        }

        [Test]
        public void AddPolygon_TooFew_HalfEdges()
        {
            var quadMesh = new QuadMesh(vertices, indices, halfEdges.ToList(), polygons);
            quadMesh.AddVertices(new List<Vertex> { new Vertex(1, 0, 2), new Vertex(3, 3, 2) });
            quadMesh.AddIndices(new List<int> { 4, 5, 1, 0 });
            var newHalfEdges = new List<HalfEdge> { new HalfEdge(vertices[4], vertices[5]), new HalfEdge(vertices[5], vertices[1]), halfEdges.First().CreateOpposite() };
            quadMesh.AddHalfEdges(newHalfEdges);

            Assert.Throws<ArgumentOutOfRangeException>(() => { quadMesh.AddPolygon(new Polygon(newHalfEdges)); });
        }
    }
}
