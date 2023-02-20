using Models.Base;

namespace Triangulator.Tests
{
    [TestFixture]
    public class SweepLinePolygonTriangulatorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Triangulate_Triangle()
        {
            var vertices = new List<Vertex2D> { new Vertex2D(0, 0), new Vertex2D(5, 0), new Vertex2D(7, 4) };
            Assert.Throws<ArgumentOutOfRangeException>(() => { var result = SweepLinePolygonTriangulator.Triangulate(vertices); });
        }

        [Test]
        public void Triangulate_Polygon()
        {
            var vertices = new List<Vertex2D> { new Vertex2D(0, 0), new Vertex2D(5, 0), new Vertex2D(7, 4), new Vertex2D(3, 6), new Vertex2D(-2, 3) };
            var result = SweepLinePolygonTriangulator.Triangulate(vertices);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(3));
                Assert.That(result.All(i => i.Count == 3), Is.True);
            });
        }

        [Test]
        public void Triangulate_Polygon_Remove_Closing_Vertex()
        {
            var vertices = new List<Vertex2D> { new Vertex2D(0, 0), new Vertex2D(5, 0), new Vertex2D(7, 4), new Vertex2D(3, 6), new Vertex2D(-2, 3), new Vertex2D(0, 0) };
            var initialVertexCount = vertices.Count;
            var result = SweepLinePolygonTriangulator.Triangulate(vertices);

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.Count.EqualTo(3));
                Assert.That(result.All(i => i.Count == 3), Is.True);
                Assert.That(result.All(i => !i.Contains(initialVertexCount - 1)), Is.True);
            });
        }
    }
}