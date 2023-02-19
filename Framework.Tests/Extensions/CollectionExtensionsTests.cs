using Framework.Extensions;

namespace Framework.Tests.Extensions
{
    [TestFixture]
    public class CollectionExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ForEach_Ok()
        {
            IEnumerable<int> items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var expectedResult = items.Select(i => i * 2).ToList();

            var result = new List<int>();
            items.ForEach(i => result.Add(i * 2));

            Assert.That(expectedResult, Is.EqualTo(result));
        }

        [Test]
        public void ForEach_Enumerable_Null()
        {
#pragma warning disable CS8600
            IEnumerable<double> items = null;
#pragma warning restore CS8600

#pragma warning disable CS8604
            Assert.Throws<ArgumentNullException>(() => items.ForEach(i => { }));
#pragma warning restore CS8604
        }

        [Test]
        public void IsCCW_IsCCW()
        {
            IList<(double X, double Y)> items = new List<(double, double)> { (0, 0), (.5, .5), (0, 1) };

            var isCCW = items.IsCCW();

            Assert.That(isCCW, Is.True);
        }

        [Test]
        public void IsCCW_IsNotCCW()
        {
            IList<(double X, double Y)> items = new List<(double, double)> { (.5, .5), (0, 0), (0, 1) };

            var isCCW = items.IsCCW();

            Assert.That(isCCW, Is.False);
        }
    }
}