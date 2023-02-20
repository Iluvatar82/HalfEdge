using Models.Base;

namespace Models.Tests.Base
{
    [TestFixture]
    public class Vertex2DTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Vertex2D_Constructor_Empty()
        {
            var vertex = new Vertex2D();

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(default(double)));
                Assert.That(vertex.Y, Is.EqualTo(default(double)));
            });
        }

        [Test]
        public void Vertex2D_Constructor_XY()
        {
            var vertex = new Vertex2D(3, 4);

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(3));
                Assert.That(vertex.Y, Is.EqualTo(4));
            });
        }

        [Test]
        public void Vertex2D_Constructor_With_ValueFunction_Two_Vertex2Ds()
        {
            var first = new Vertex2D(8, -6);
            var second = new Vertex2D(-8, 6);

            var vertex = new Vertex2D(first, second, Math.Min);

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(-8));
                Assert.That(vertex.Y, Is.EqualTo(-6));
            });
        }

        [Test]
        public void Vertex2D_Implicit_Convert_From_Vertex()
        {
            var vertex3D = new Vertex(7, 4, 1);
            Vertex2D vertex = vertex3D;

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(7));
                Assert.That(vertex.Y, Is.EqualTo(4));
            });
        }

        [Test]
        public void Vertex2D_Implicit_Convert_From_Vector2D()
        {
            Vertex2D vertex = new Vector2D(8, -6);

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(8));
                Assert.That(vertex.Y, Is.EqualTo(-6));
            });
        }

        [Test]
        public void DoubleArray_Implicit_Convert_From_Vertex2D()
        {
            var vertex = new Vertex2D(3, 7);
            double[] vertexData = vertex;

            Assert.Multiple(() =>
            {
                Assert.That(vertexData[0], Is.EqualTo(3));
                Assert.That(vertexData[1], Is.EqualTo(7));
            });
        }

        [Test]
        public void Vertex2D_Implicit_Convert_From_DoubleArray_Ok()
        {
            double[] vertexData = new double[] { 4, 5 };
            Vertex2D vertex = vertexData;

            Assert.Multiple(() =>
            {
                Assert.That(vertexData[0], Is.EqualTo(4));
                Assert.That(vertexData[1], Is.EqualTo(5));
            });
        }

        [Test]
        public void Vertex2D_Implicit_Convert_From_DoubleArray_Not_Ok()
        {
            double[] vertexData = new double[] { 4, 5, 9, 28 };
            Assert.Throws<ArgumentOutOfRangeException>(() => { Vertex2D vertex = vertexData; });
        }

        [Test]
        public void Vertex2D_Operator_Minus()
        {
            var vertex1 = new Vertex2D(2, 2);
            var vertex2 = new Vertex2D(-1, -3);

            var vertex = vertex1 - vertex2;

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(3));
                Assert.That(vertex.Y, Is.EqualTo(5));
            });
        }

        [Test]
        public void Vertex2D_Operator_Plus()
        {
            var vertex1 = new Vertex2D(2, 2);
            var vertex2 = new Vertex2D(-1, -3);

            var vertex = vertex1 + vertex2;

            Assert.Multiple(() =>
            {
                Assert.That(vertex.X, Is.EqualTo(1));
                Assert.That(vertex.Y, Is.EqualTo(-1));
            });
        }
    }
}
