using Models.Base;

namespace Models.Tests
{
    [TestFixture]
    public class PolygonTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreatePolygon_Empty_Ok()
        {
            var polygon = new Polygon();
            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Is.Empty);
                Assert.That(polygon.Vertices.ToList(), Is.Empty);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.IsBorder, Is.True);
            });
        }

        [Test]
        public void CreatePolygon_from_Vertices_Ok()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 1, 1);
            var vertex3 = new Vertex(0, 1, 1);

            var polygon = new Polygon(new List<Vertex> { vertex1, vertex2, vertex3 });
            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(polygon.Vertices.ToList(), Has.Count.EqualTo(3));
                Assert.That(polygon.HalfEdges.Select(h => h.Next), Has.All.Not.Null);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.HalfEdges.Select(h => h.Polygon), Has.All.EqualTo(polygon));
                Assert.That(polygon.IsBorder, Is.True);
                Assert.That(polygon.Vertices.Select(v => v.Polygons.ToList().Count), Has.All.EqualTo(1));
                Assert.That(polygon.Vertices.Select(v => v.VertexNeighbors.ToList().Count), Has.All.EqualTo(2));
            });
        }

        [Test]
        public void CreatePolygon_from_Vertices_Not_Ok()
        {
            var vertex = new Vertex(4, 3, 2);

            Assert.Throws<ArgumentOutOfRangeException>(() => { var polygon = new Polygon(new List<Vertex> { vertex }); });
        }

        [Test]
        public void CreatePolygon_from_HalfEdges_Ok()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 1, 1);
            var vertex3 = new Vertex(0, 1, 1);

            var halfEdge1 = new HalfEdge(vertex1, vertex2);
            var halfEdge2 = new HalfEdge(vertex2, vertex3);
            var halfEdge3 = new HalfEdge(vertex3, vertex1);

            var polygon = new Polygon(new[] { halfEdge1, halfEdge2, halfEdge3 });
            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(polygon.Vertices.ToList(), Has.Count.EqualTo(3));
                Assert.That(polygon.HalfEdges.Select(h => h.Next), Has.All.Not.Null);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.HalfEdges.Select(h => h.Polygon), Has.All.EqualTo(polygon));
                Assert.That(polygon.IsBorder, Is.True);
                Assert.That(polygon.Vertices.Select(v => v.Polygons.ToList().Count), Has.All.EqualTo(1));
                Assert.That(polygon.Vertices.Select(v => v.VertexNeighbors.ToList().Count), Has.All.EqualTo(2));
            });
        }

        [Test]
        public void CreatePolygon_from_HalfEdges_NotComplete()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 1, 1);
            var vertex3 = new Vertex(0, 1, 1);

            var halfEdge1 = new HalfEdge(vertex1, vertex2);
            var halfEdge2 = new HalfEdge(vertex2, vertex3);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Polygon(new[] { halfEdge1, halfEdge2 }));
        }

        [Test]
        public void Deconstruct_Polygon_Ok()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 0, 1);
            var vertex3 = new Vertex(1, 1, 1);
            var vertex4 = new Vertex(0, 1, 0);

            var polygon = new Polygon(new List<Vertex> { vertex1, vertex2, vertex3, vertex4 });
            var (vertices, halfEdges) = polygon;

            Assert.Multiple(() =>
            {
                Assert.That(vertices.ToList(), Has.Count.EqualTo(4));
                Assert.That(halfEdges.ToList(), Has.Count.EqualTo(4));
            });
        }

        [Test]
        public void Polygon_Conversion_Implicit_to_Vertices()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 1, 1);
            var vertex3 = new Vertex(0, 1, 1);

            var polygon = new Polygon(new List<Vertex> { vertex1, vertex2, vertex3 });
            Vertex[] vertices = polygon;

            Assert.That(vertices, Has.Length.EqualTo(3));
        }

        [Test]
        public void Polygon_Conversion_Implicit_to_HalfEdges()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 1, 1);
            var vertex3 = new Vertex(0, 1, 1);

            var polygon = new Polygon(new List<Vertex> { vertex1, vertex2, vertex3 });
            HalfEdge[] halfEdges = polygon;

            Assert.That(halfEdges, Has.Length.EqualTo(3));
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_Ok()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 1, 1);
            var vertex3 = new Vertex(0, 1, 1);
            Polygon polygon = new Vertex[] { vertex1, vertex2, vertex3 };

            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(polygon.Vertices.ToList(), Has.Count.EqualTo(3));
                Assert.That(polygon.HalfEdges.Select(h => h.Next), Has.All.Not.Null);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.HalfEdges.Select(h => h.Polygon), Has.All.EqualTo(polygon));
                Assert.That(polygon.IsBorder, Is.True);
                Assert.That(polygon.Vertices.Select(v => v.Polygons.ToList().Count), Has.All.EqualTo(1));
                Assert.That(polygon.Vertices.Select(v => v.VertexNeighbors.ToList().Count), Has.All.EqualTo(2));
            });
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_Null()
        {
            Vertex[]? vertexArray = null;
#pragma warning disable CS8604
            Assert.Throws<ArgumentNullException>(() => { Polygon polygon = vertexArray; });
#pragma warning restore CS8604
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_Empty()
        {
            var vertexArray = new Vertex[] { };
            Assert.Throws<ArgumentOutOfRangeException>(() => { Polygon polygon = vertexArray; });
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_too_few_Elements()
        {
            var vertex1 = new Vertex(0, 0, 0);
            var vertex2 = new Vertex(1, 1, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => { Polygon polygon = new[] { vertex1, vertex2 }; });
        }
    }
}
