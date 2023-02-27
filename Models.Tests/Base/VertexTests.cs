using Framework;
using Models.Base;

namespace Models.Tests.Base
{
    [TestFixture]
    public class VertexTests
    {
        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void Create_Vertex()
        {
            var vertex = new Vertex(1, 1, 1);
            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(1));
                Assert.That(vertex.Y, Is.EqualTo(1));
                Assert.That(vertex.Z, Is.EqualTo(1));
                Assert.That(vertex.HalfEdges, Is.Empty);
                Assert.That(vertex.Polygons, Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Create_Empty_Vertex()
        {
            var vertex = new Vertex();
            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(default(double)));
                Assert.That(vertex.Y, Is.EqualTo(default(double)));
                Assert.That(vertex.Z, Is.EqualTo(default(double)));
                Assert.That(vertex.HalfEdges.ToList(), Is.Empty);
                Assert.That(vertex.Polygons.ToList(), Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Deconstruct_Vertex_Split()
        {
            var vertex = new Vertex(2, 4, 8);
            var (x, y, z) = vertex;

            Assert.Multiple(() =>
            {
                Assert.That(x, Is.EqualTo(vertex.X));
                Assert.That(y, Is.EqualTo(vertex.Y));
                Assert.That(z, Is.EqualTo(vertex.Z));
                Assert.That(vertex.HalfEdges, Is.Empty);
                Assert.That(vertex.Polygons, Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Create_Vertex_With_ValueFunction()
        {
            var vertex1 = new Vertex(1, -2, 3);
            var vertex2 = new Vertex(-1, 4, 9);

            var vertex = new Vertex(vertex1, vertex2, Average);

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(0));
                Assert.That(vertex.Y, Is.EqualTo(1));
                Assert.That(vertex.Z, Is.EqualTo(6));
                Assert.That(vertex.HalfEdges, Is.Empty);
                Assert.That(vertex.Polygons, Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Create_Vertex_With_AggregateFunction()
        {
            var vertex1 = new Vertex(1, -2, 3);
            var vertex2 = new Vertex(-1, 4, 9);
            var vertex3 = new Vertex(3, 2, -1);

            var vertex = new Vertex((vl) => vl.Sum(), new[] { vertex1, vertex2, vertex3 });

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(3));
                Assert.That(vertex.Y, Is.EqualTo(4));
                Assert.That(vertex.Z, Is.EqualTo(11));
                Assert.That(vertex.HalfEdges, Is.Empty);
                Assert.That(vertex.Polygons, Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Vertex_Conversion_Implicit_Vertex2D_to_Vertex()
        {
            Vertex vertex = new Vertex2D(2, 4);

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(2));
                Assert.That(vertex.Y, Is.EqualTo(4));
                Assert.That(vertex.Z, Is.EqualTo(0));
                Assert.That(vertex.HalfEdges, Is.Empty);
                Assert.That(vertex.Polygons, Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Vertex_Conversion_Implicit_Vertex_to_Array()
        {
            var vertex = new Vertex(2, 4, 8);
            double[] vertexData = vertex;

            Assert.Multiple(() =>
            {
                Assert.That(vertexData, Has.Length.EqualTo(3));
                Assert.That(vertexData[0], Is.EqualTo(vertex.X));
                Assert.That(vertexData[1], Is.EqualTo(vertex.Y));
                Assert.That(vertexData[2], Is.EqualTo(vertex.Z));
                Assert.That(vertex.HalfEdges, Is.Empty);
                Assert.That(vertex.Polygons, Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Vertex_Conversion_Implicit_Array_to_Vertex()
        {
            var vertexData = new[] { 1d, 2d, 3d };
            Vertex vertex = vertexData;

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(vertexData[0]));
                Assert.That(vertex.Y, Is.EqualTo(vertexData[1]));
                Assert.That(vertex.Z, Is.EqualTo(vertexData[2]));
                Assert.That(vertex.HalfEdges, Is.Empty);
                Assert.That(vertex.Polygons, Is.Empty);
                Assert.That(vertex.VertexNeighbors, Is.Empty);
                Assert.That(vertex.IsBorder, Is.True);
            });
        }

        [Test]
        public void Vertex_Conversion_Implicit_Array_to_Vertex_Array_Not_Ok()
        {
            var vertexData = new[] { 1d, 2d, 3d, 4d, 5d };
            Assert.Throws<ArgumentOutOfRangeException>(() => { Vertex vertex = vertexData; });
        }

        [Test]
        public void Vertex_SquaredDistanceTo_Zero()
        {
            var vertex1 = new Vertex(5d, 5d, 3d);
            var vertex2 = new Vertex(5d, 5d, 3d);

            var distance = vertex1.SquaredDistanceTo(vertex2);

            Assert.That(distance, Is.EqualTo(0).Within(Constants.Epsilon));
        }

        [Test]
        public void Vertex_SquaredDistanceTo()
        {
            var vertex1 = new Vertex(5d, 5d, 3d);
            var vertex2 = new Vertex(0d, 5d, 3d);

            var distance = vertex1.SquaredDistanceTo(vertex2);

            Assert.That(distance, Is.EqualTo(25).Within(Constants.Epsilon));
        }

        [Test]
        public void Vertex_DistanceTo()
        {
            var vertex1 = new Vertex(5d, 5d, 3d);
            var vertex2 = new Vertex(-2d, 5d, 3d);

            var distance = vertex1.DistanceTo(vertex2);

            Assert.That(distance, Is.EqualTo(7).Within(Constants.Epsilon));
        }

        private double Average(double first, double second) => (first + second) * .5;
    }
}