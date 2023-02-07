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
        public void CreatePolygon_Test()
        {
            var vertex1 = new Vertex<double>(0, 0, 0);
            var vertex2 = new Vertex<double>(1, 1, 1);
            var vertex3 = new Vertex<double>(0, 1, 1);

            var halfEdge1 = new HalfEdge<double>(vertex1, vertex2);
            var halfEdge2 = new HalfEdge<double>(vertex2, vertex3);
            var halfEdge3 = new HalfEdge<double>(vertex3, vertex1);

            var polygon = new Polygon<double>(new [] { halfEdge1, halfEdge2, halfEdge3 });
            Assert.Multiple(() =>
            {
                Assert.That(polygon.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(polygon.Vertices.ToList(), Has.Count.EqualTo(3));
                Assert.That(polygon.HalfEdges.Select(h => h.Next), Has.All.Not.Null);
                Assert.That(polygon.Neighbors, Is.Empty);
                Assert.That(polygon.HalfEdges.Select(h => h.Polygon), Has.All.EqualTo(polygon));
                Assert.That(polygon.IsBorder, Is.True);
            });
        }
    }
}
