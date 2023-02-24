using Models.Base;

namespace Models.Tests
{
    [TestFixture]
    public class AxisAlignedBoundingBoxTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Construct_AxisAlignedBoundingBoxTests_Null()
        {
#pragma warning disable CS8625
            Assert.Throws<ArgumentNullException>(() => { var bb = new AxisAlignedBoundingBox(null); });
#pragma warning restore CS8625
        }

        [Test]
        public void Construct_AxisAlignedBoundingBoxTests_No_Vertex()
        {
            var vertices = new List<Vertex>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { var bb = new AxisAlignedBoundingBox(vertices); });
        }

        [Test]
        public void Construct_AxisAlignedBoundingBoxTests_OneVertex()
        {
            var vertices = new List<Vertex> { new Vertex(2, 3, 4) };
            var bb = new AxisAlignedBoundingBox(vertices);

            Assert.Multiple(() =>
            {
                Assert.That(bb.Min.X, Is.EqualTo(2));
                Assert.That(bb.Min.Y, Is.EqualTo(3));
                Assert.That(bb.Min.Z, Is.EqualTo(4));
                Assert.That(bb.Max.X, Is.EqualTo(bb.Min.X));
                Assert.That(bb.Max.Y, Is.EqualTo(bb.Min.Y));
                Assert.That(bb.Max.Z, Is.EqualTo(bb.Min.Z));
            });
        }

        [Test]
        public void Construct_AxisAlignedBoundingBoxTests_Multiple_Vertices()
        {
            var vertices = new List<Vertex> { new Vertex(2, 3, 4), new Vertex(-3, 2, 5) };
            var bb = new AxisAlignedBoundingBox(vertices);

            Assert.Multiple(() =>
            {
                Assert.That(bb.Min.X, Is.EqualTo(-3));
                Assert.That(bb.Min.Y, Is.EqualTo(2));
                Assert.That(bb.Min.Z, Is.EqualTo(4));
                Assert.That(bb.Max.X, Is.EqualTo(2));
                Assert.That(bb.Max.Y, Is.EqualTo(3));
                Assert.That(bb.Max.Z, Is.EqualTo(5));
            });
        }
    }
}
