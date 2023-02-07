using HalfEdge.Models;

namespace HalfEdge.Tests.Models
{
    [TestFixture]
    public class Vertex
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void CreateVertex_Test()
        {
            var vertex = new Vertex<double>(1, 1, 1);
            Assert.That(vertex.X, Is.EqualTo(1));
            Assert.That(vertex.Y, Is.EqualTo(1));
            Assert.That(vertex.Z, Is.EqualTo(1));
            Assert.That(vertex.OutHalfEdges.Count, Is.EqualTo(0));
        }
    }
}