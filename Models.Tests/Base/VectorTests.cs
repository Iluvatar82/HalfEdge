using Framework;
using Models.Base;

namespace Models.Tests.Base
{
    [TestFixture]
    public class VectorTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Vector_Constructor_Empty()
        {
            var vector = new Vector();

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(default(double)));
                Assert.That(vector.Y, Is.EqualTo(default(double)));
                Assert.That(vector.Z, Is.EqualTo(default(double)));
                Assert.That(vector.Length, Is.EqualTo(default(double)));
            });
        }

        [Test]
        public void Vector_Constructor_XYZ()
        {
            var vector = new Vector(3, 4, 5);

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(3));
                Assert.That(vector.Y, Is.EqualTo(4));
                Assert.That(vector.Z, Is.EqualTo(5));
                Assert.That(vector.Length, Is.EqualTo(Math.Sqrt(50)));
            });
        }

        [Test]
        public void Vector_Constructor_With_Action_From_Two_Vertex()
        {
            var first = new Vertex(8, -6, 4);
            var second = new Vertex(-8, 6, -2);

            var vector = new Vector(first, second, Math.Min);

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(-8));
                Assert.That(vector.Y, Is.EqualTo(-6));
                Assert.That(vector.Z, Is.EqualTo(-2));
                Assert.That(vector.Length, Is.EqualTo(Math.Sqrt(104)));
            });
        }

        [Test]
        public void Vector_Implicit_Convert_From_Vertex()
        {
            Vector vector = new Vertex(8, -6, 4);

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(8));
                Assert.That(vector.Y, Is.EqualTo(-6));
                Assert.That(vector.Z, Is.EqualTo(4));
                Assert.That(vector.Length, Is.EqualTo(Math.Sqrt(116)));
            });
        }

        [Test]
        public void Vector_Operator_Minus()
        {
            var vector1 = new Vector(2, 2, 8);
            var vector2 = new Vector(-1, -3, -3);

            var vector = vector1 - vector2;

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(3));
                Assert.That(vector.Y, Is.EqualTo(5));
                Assert.That(vector.Z, Is.EqualTo(11));
                Assert.That(vector.Length, Is.EqualTo(Math.Sqrt(155)));
            });
        }

        [Test]
        public void Vector_Operator_Plus()
        {
            var vector1 = new Vector(2, 2, 3);
            var vector2 = new Vector(-1, -3, -2);

            var vector = vector1 + vector2;

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(1));
                Assert.That(vector.Y, Is.EqualTo(-1));
                Assert.That(vector.Z, Is.EqualTo(1));
                Assert.That(vector.Length, Is.EqualTo(Math.Sqrt(3)));
            });
        }

        [Test]
        public void Vector_Operator_Multiply()
        {
            var vector = new Vector(2, 2, 3);
            var result = 2.5 * vector * 0.8;

            Assert.Multiple(() =>
            {
                Assert.That(result.X, Is.EqualTo(4));
                Assert.That(result.Y, Is.EqualTo(4));
                Assert.That(result.Z, Is.EqualTo(6));
                Assert.That(result.Length, Is.EqualTo(Math.Sqrt(68)));
            });
        }

        [Test]
        public void Vector_Operator_Divide_Ok()
        {
            var vector = new Vector(5, -2, 8);
            var result = vector / 4;

            Assert.Multiple(() =>
            {
                Assert.That(result.X, Is.EqualTo(1.25));
                Assert.That(result.Y, Is.EqualTo(-.5));
                Assert.That(result.Z, Is.EqualTo(2));
                Assert.That(result.Length, Is.EqualTo(Math.Sqrt(5.8125)));
            });
        }

        [Test]
        public void Vector_Operator_Divide_Zero()
        {
            var vector = new Vector(5, -2, 8);
            Assert.Throws<ArgumentOutOfRangeException>(() => _ = vector / 0);
        }

        [Test]
        public void Vector_Normalize()
        {
            var vector = new Vector(2, 2, 2);
            vector.Normalize();

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(2 / Math.Sqrt(12)).Within(Constants.Epsilon));
                Assert.That(vector.Y, Is.EqualTo(2 / Math.Sqrt(12)).Within(Constants.Epsilon));
                Assert.That(vector.Z, Is.EqualTo(2 / Math.Sqrt(12)).Within(Constants.Epsilon));
                Assert.That(vector.Length, Is.EqualTo(1));
            });
        }
    }
}
