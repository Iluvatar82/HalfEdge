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
            IEnumerable<int> _items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var expectedResult = _items.Select(i => i * 2).ToList();

            var result = new List<int>();
            _items.ForEach(i => result.Add(i * 2));

            Assert.That(expectedResult, Is.EqualTo(result));
        }

        [Test]
        public void ForEach_Enumerable_Null()
        {
#pragma warning disable CS8600
            IEnumerable<double> _items = null;
#pragma warning restore CS8600

#pragma warning disable CS8604
            Assert.Throws<ArgumentNullException>(() => _items.ForEach(i => { }));
#pragma warning restore CS8604
        }
    }
}