using HalfEdge.Models;

namespace HalfEdge.Tests.Models
{
    [TestFixture]
    public class Polygon
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CreatePolygon_Empty_Ok()
        {
            var polygon = new Polygon<double>();
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
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);
            var vertex3 = new Vertex<double>(0, 1, 1);

            var polygon = new Polygon<double>(new List<Vertex<double>> { vertex1, vertex2, vertex3 });
            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(polygon.Vertices.ToList(), Has.Count.EqualTo(3));
                Assert.That(polygon.HalfEdges.Select(h => h.Next), Has.All.Not.Null);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.HalfEdges.Select(h => h.Polygon), Has.All.EqualTo(polygon));
                Assert.That(polygon.IsBorder, Is.True);
                Assert.That(polygon.Vertices.Select(v => v.Polygons.ToList().Count), Has.All.EqualTo(1));
            });
        }

        [Test]
        public void CreatePolygon_from_HalfEdges_Ok()
        {
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);
            var vertex3 = new Vertex<double>(0, 1, 1);

            var halfEdge1 = new HalfEdge<double>(vertex1, vertex2);
            var halfEdge2 = new HalfEdge<double>(vertex2, vertex3);
            var halfEdge3 = new HalfEdge<double>(vertex3, vertex1);

            var polygon = new Polygon<double>(new[] { halfEdge1, halfEdge2, halfEdge3 });
            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(polygon.Vertices.ToList(), Has.Count.EqualTo(3));
                Assert.That(polygon.HalfEdges.Select(h => h.Next), Has.All.Not.Null);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.HalfEdges.Select(h => h.Polygon), Has.All.EqualTo(polygon));
                Assert.That(polygon.IsBorder, Is.True);
                Assert.That(polygon.Vertices.Select(v => v.Polygons.ToList().Count), Has.All.EqualTo(1));
            });
        }

        [Test]
        public void CreatePolygon_from_HalfEdges_NotComplete()
        {
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);
            var vertex3 = new Vertex<double>(0, 1, 1);

            var halfEdge1 = new HalfEdge<double>(vertex1, vertex2);
            var halfEdge2 = new HalfEdge<double>(vertex2, vertex3);

            Assert.Throws<ArgumentOutOfRangeException>(() => new Polygon<double>(new[] { halfEdge1, halfEdge2 }));
        }

        [Test]
        public void Deconstruct_Polygon_Ok()
        {
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 0, 1);
            var vertex3 = new Vertex<double>(1, 1, 1);
            var vertex4 = new Vertex<double>(0, 1, 0);

            var polygon = new Polygon<double>(new List<Vertex<double>> { vertex1, vertex2, vertex3, vertex4 });
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
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);
            var vertex3 = new Vertex<double>(0, 1, 1);

            var polygon = new Polygon<double>(new List<Vertex<double>> { vertex1, vertex2, vertex3 });
            Vertex<double>[] vertices = polygon;

            Assert.That(vertices, Has.Length.EqualTo(3));
        }

        [Test]
        public void Polygon_Conversion_Implicit_to_HalfEdges()
        {
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);
            var vertex3 = new Vertex<double>(0, 1, 1);

            var polygon = new Polygon<double>(new List<Vertex<double>> { vertex1, vertex2, vertex3 });
            HalfEdge<double>[] halfEdges = polygon;

            Assert.That(halfEdges, Has.Length.EqualTo(3));
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_Ok()
        {
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);
            var vertex3 = new Vertex<double>(0, 1, 1);
            Polygon<double> polygon = new Vertex<double>[] { vertex1, vertex2, vertex3 };

            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(polygon.Vertices.ToList(), Has.Count.EqualTo(3));
                Assert.That(polygon.HalfEdges.Select(h => h.Next), Has.All.Not.Null);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.HalfEdges.Select(h => h.Polygon), Has.All.EqualTo(polygon));
                Assert.That(polygon.IsBorder, Is.True);
                Assert.That(polygon.Vertices.Select(v => v.Polygons.ToList().Count), Has.All.EqualTo(1));
            });
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_Null()
        {
            Vertex<double>[]? vertexArray = null;
#pragma warning disable CS8604 // Mögliches Nullverweisargument.
            Assert.Throws<ArgumentNullException>(() => { Polygon<double> polygon = vertexArray; });
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_Empty()
        {
            var vertexArray = new Vertex<double>[] { };
            Assert.Throws<ArgumentOutOfRangeException>(() => { Polygon<double> polygon = vertexArray; });
        }

        [Test]
        public void Polygon_Conversion_Implicit_VertexArray_to_Polygon_too_few_Elements()
        {
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => { Polygon<double> polygon = new[] { vertex1, vertex2 }; });
        }
    }
}
