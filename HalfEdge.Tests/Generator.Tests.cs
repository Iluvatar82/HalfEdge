using HalfEdge.Models;

namespace HalfEdge.Tests
{
    public class Generator
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void CreateGenerator_Test()
        {
            var generator = new Generator<double>(new List<Vertex<double>>(), new List<List<int>>());
            Assert.Multiple(() =>
            {
                Assert.That(generator.Vertices, Is.Empty);
                Assert.That(generator.Indices, Is.Empty);
                Assert.That(generator.Mesh.HalfEdges, Is.Empty);
            });
        }

        [Test]
        public void CreateGenerator_WithVertices_Test()
        {
            var vertices = new List<Vertex<double>> { new Vertex<double>(0, 0, 0), new Vertex<double>(1, 1, 1), new Vertex<double>(0, 1, 1) };
            var generator = new Generator<double>(vertices, new List<List<int>>());
            Assert.Multiple(() =>
            {
                Assert.That(generator.Vertices, Is.Not.Empty);
                Assert.That(generator.Vertices, Has.Count.EqualTo(3));
                Assert.That(generator.Indices, Is.Empty);
                Assert.That(generator.Mesh.HalfEdges, Is.Empty);
            });
        }

        [Test]
        public void CreateGenerator_WithOnePolygon_Test()
        {
            var vertices = new List<Vertex<double>> { new Vertex<double>(0, 0, 0), new Vertex<double>(1, 1, 1), new Vertex<double>(0, 1, 1) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 } };
            var generator = new Generator<double>(vertices, indices);
            Assert.Multiple(() =>
            {
                Assert.That(generator.Vertices, Is.Not.Empty);
                Assert.That(generator.Vertices, Has.Count.EqualTo(3));
                Assert.That(generator.Indices, Is.Not.Empty);
                Assert.That(generator.Indices, Has.Count.EqualTo(1));
                Assert.That(generator.Mesh.HalfEdges, Is.Not.Empty);
                Assert.That(generator.Mesh.HalfEdges, Has.Count.EqualTo(3));
                Assert.That(generator.Mesh.HalfEdges.All(h => h.IsBorder), Is.True);
                Assert.That(generator.PolygonCount, Is.EqualTo(1));
            });
        }

        [Test]
        public void CreateGenerator_WithTwoPolygon_Test()
        {
            var vertices = new List<Vertex<double>> { new Vertex<double>(0, 0, 0), new Vertex<double>(1, 0, 0), new Vertex<double>(1, 1, 1), new Vertex<double>(0, 1, 1) };
            var indices = new List<List<int>> { new List<int> { 0, 1, 2 }, new List<int> { 0, 2, 3 } };
            var generator = new Generator<double>(vertices, indices);
            Assert.Multiple(() =>
            {
                Assert.That(generator.Vertices, Is.Not.Empty);
                Assert.That(generator.Vertices, Has.Count.EqualTo(4));
                Assert.That(generator.Indices, Is.Not.Empty);
                Assert.That(generator.Indices, Has.Count.EqualTo(2));
                Assert.That(generator.Mesh.HalfEdges, Is.Not.Empty);
                Assert.That(generator.Mesh.HalfEdges, Has.Count.EqualTo(6));
                Assert.That(generator.PolygonCount, Is.EqualTo(2));
                Assert.That(generator.Mesh.HalfEdges.Count(h => h.Opposite is not null), Is.EqualTo(2));
                Assert.That(generator.Mesh.HalfEdges.Count(h => h.IsBorder), Is.EqualTo(4));
            });
        }
    }
}
