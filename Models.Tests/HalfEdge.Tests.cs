using Models.Base;
using Validation;

namespace Models.Tests
{
    [TestFixture]
    public class HalfEdge
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void Create_HalfEdge()
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
        public void Create_Empty_HalfEdge()
        {
            var halfEdge = new HalfEdge<double>();
            Assert.Multiple(() =>
            {
                Assert.That(halfEdge.Start, Is.Not.Null);
                Assert.That(halfEdge.Start.X, Is.EqualTo(0));
                Assert.That(halfEdge.Start.Y, Is.EqualTo(0));
                Assert.That(halfEdge.Start.Z, Is.EqualTo(0));
                Assert.That(halfEdge.End, Is.Not.Null);
                Assert.That(halfEdge.End.X, Is.EqualTo(0));
                Assert.That(halfEdge.End.Y, Is.EqualTo(0));
                Assert.That(halfEdge.End.Z, Is.EqualTo(0));
                Assert.That(halfEdge.Opposite, Is.Null);
                Assert.That(halfEdge.Previous, Is.Null);
                Assert.That(halfEdge.Next, Is.Null);
                Assert.That(halfEdge.Polygon, Is.Null);
                Assert.That(halfEdge.Start.HalfEdges, Is.Empty);
                Assert.That(halfEdge.End.HalfEdges, Is.Empty);
                Assert.That(halfEdge.IsBorder, Is.True);
            });
        }


        [Test]
        public void Create_HalfEdge_Opposite()
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

        [Test]
        public void Deconstruct_HalfEdge_Split()
        {
            var vertexStart = new Vertex<double>(0, 0, 0);
            var vertexEnd = new Vertex<double>(1, 1, 1);

            var halfEdge = new HalfEdge<double>(vertexStart, vertexEnd);
            var (start, end) = halfEdge;

            Assert.Multiple(() =>
            {
                Assert.That(start, Is.EqualTo(vertexStart));
                Assert.That(end, Is.EqualTo(vertexEnd));
            });
        }

        [Test]
        public void HalfEdge_Conversion_Implicit_Start_End_to_HalfEdge()
        {
            var vertexStart = new Vertex<double>(0, 0, 0);
            var vertexEnd = new Vertex<double>(1, 1, 1);

            HalfEdge<double> halfEdge = (vertexStart, vertexEnd);

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
    }
}
