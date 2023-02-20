using Framework;
using Models.Base;

namespace Models.Tests.Base
{
    [TestFixture]
    public class Vector2DTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Vector2D_Constructor_Empty()
        {
            var vector = new Vector2D();

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(default(double)));
                Assert.That(vector.Y, Is.EqualTo(default(double)));
                Assert.That(vector.Length, Is.EqualTo(default(double)));
            });
        }

        [Test]
        public void Vector2D_Constructor_XY_L_5()
        {
            var vector = new Vector2D(3, 4);

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(3));
                Assert.That(vector.Y, Is.EqualTo(4));
                Assert.That(vector.Length, Is.EqualTo(5));
            });
        }

        [Test]
        public void Vector2D_Constructor_With_Action_From_Two_Vertex2Ds()
        {
            var first = new Vertex2D(8, -6);
            var second = new Vertex2D(-8, 6);

            var vector = new Vector2D(first, second, Math.Min); 

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(-8));
                Assert.That(vector.Y, Is.EqualTo(-6));
                Assert.That(vector.Length, Is.EqualTo(10));
            });
        }

        [Test]
        public void Vector2D_Implicit_Convert_From_Vertex()
        {
            Vector2D vector = new Vertex2D(8, -6);

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(8));
                Assert.That(vector.Y, Is.EqualTo(-6));
                Assert.That(vector.Length, Is.EqualTo(10));
            });
        }

        [Test]
        public void Vector2D_Operator_Minus()
        {
            var vector1 = new Vector2D(2, 2);
            var vector2 = new Vector2D(-1, -3);

            var vector = vector1 - vector2;

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(3));
                Assert.That(vector.Y, Is.EqualTo(5));
                Assert.That(vector.Length, Is.EqualTo(Math.Sqrt(34)));
            });
        }

        [Test]
        public void Vector2D_Operator_Plus()
        {
            var vector1 = new Vector2D(2, 2);
            var vector2 = new Vector2D(-1, -3);

            var vector = vector1 + vector2;

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(1));
                Assert.That(vector.Y, Is.EqualTo(-1));
                Assert.That(vector.Length, Is.EqualTo(Math.Sqrt(2)));
            });
        }

        [Test]
        public void Vector2D_Normalize()
        {
            var vector = new Vector2D(2, 2);
            vector.Normalize();

            Assert.Multiple(() =>
            {
                Assert.That(vector.X, Is.EqualTo(Math.Sqrt(2) / 2).Within(Constants.Epsilon));
                Assert.That(vector.Y, Is.EqualTo(Math.Sqrt(2) / 2).Within(Constants.Epsilon));
                Assert.That(vector.Length, Is.EqualTo(1));
            });
        }
    }
}
