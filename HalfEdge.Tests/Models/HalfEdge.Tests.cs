using HalfEdge.Models;
using Validation;

namespace HalfEdge.Tests.Models
{
    [TestFixture]
    public class HalfEdge
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void CreateHalfEdge_Test()
        {
            var vertexStart = new Vertex<double>(0, 0, 0);
            var vertexEnd = new Vertex<double>(1, 1, 1);

            var halfEdge = new HalfEdge<double>(vertexStart, vertexEnd);
            
            Assert.Multiple(() =>
            {
                Assert.That(halfEdge.Start, Is.EqualTo(vertexStart));
                Assert.That(halfEdge.End, Is.EqualTo(vertexEnd));
                Assert.That(halfEdge.Opposite, Is.Null);
                Assert.That(halfEdge.Previous, Is.Null);
                Assert.That(halfEdge.Next, Is.Null);
                Assert.That(halfEdge.Polygon, Is.Null);
                Assert.That(halfEdge.Start.HalfEdges, Has.Count.EqualTo(1));
                Assert.That(halfEdge.Start.HalfEdges[0], Is.EqualTo(halfEdge));
                Assert.That(halfEdge.End.HalfEdges, Is.Empty);
                Assert.That(halfEdge.IsBorder, Is.True);
            });
        }

        [Test]
        public void CreateHalfEdge_Opposite_Test()
        {
            var vertexStart = new Vertex<double>(0, 0, 0);
            var vertexEnd = new Vertex<double>(1, 1, 1);

            var halfEdge = new HalfEdge<double>(vertexStart, vertexEnd);
            var halfEdgeOpposite = halfEdge.CreateOpposite();
            
            Assert.Multiple(() =>
            {
                Assert.That(halfEdgeOpposite.Start, Is.EqualTo(vertexEnd));
                Assert.That(halfEdgeOpposite.End, Is.EqualTo(vertexStart));
                Assert.That(halfEdgeOpposite.Opposite, Is.EqualTo(halfEdge));
                Assert.That(halfEdge.Previous, Is.Null);
                Assert.That(halfEdge.Next, Is.Null);
                Assert.That(halfEdge.Polygon, Is.Null);
                Assert.That(halfEdgeOpposite.Polygon, Is.Null);
                Assert.That(halfEdgeOpposite.Start.HalfEdges, Has.Count.EqualTo(1));
                Assert.That(halfEdgeOpposite.Start.HalfEdges[0], Is.EqualTo(halfEdgeOpposite));
                Assert.That(halfEdgeOpposite.End.HalfEdges, Has.Count.EqualTo(1));
                Assert.That(halfEdgeOpposite.End.HalfEdges[0], Is.EqualTo(halfEdge));
                Assert.That(halfEdgeOpposite.Opposite?.Opposite, Is.EqualTo(halfEdgeOpposite));
                Assert.That(halfEdge.IsBorder, Is.False);
                Assert.That(halfEdgeOpposite.IsBorder, Is.False);
            });
        }
    }
}
