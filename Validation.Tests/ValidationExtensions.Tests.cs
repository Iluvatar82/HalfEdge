using Models.Base;

namespace Validation.Tests
{
    [TestFixture]
    public class ValidationExtensions
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Validate_Not_Null_Ok()
        {
            var myString = "The is a typical String!";
            myString.NotNull();

            Assert.Pass();
        }

        [Test]
        public void Validate_Null()
        {
            Vertex<int>? myVertex = null;
            Assert.Throws<ArgumentNullException>(() => myVertex.NotNull());
        }

        [Test]
        public void Validate_Enumerable_Not_Empty_Ok()
        {
            var myEnumerable = new[] { 2, 3, 4 };
            myEnumerable.NotEmpty();

            Assert.Pass();
        }

        [Test]
        public void Validate_Enumerable_Empty()
        {
            var myEnumerable = Array.Empty<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => myEnumerable.NotEmpty());
        }

        [Test]
        public void Validate_Enumerable_Not_Null_Or_Empty_Ok()
        {
            var myEnumerable = new[] { 2, 3, 4 };
            myEnumerable.NotNullOrEmpty();

            Assert.Pass();
        }

        [Test]
        public void Validate_Enumerable_Not_Null_Or_Empty_Null()
        {
            int[]? myEnumerable = null;

#pragma warning disable CS8604 // Mögliches Nullverweisargument.
            Assert.Throws<ArgumentNullException>(() => myEnumerable.NotNullOrEmpty());
#pragma warning restore CS8604 // Mögliches Nullverweisargument.
        }

        [Test]
        public void Validate_Enumerable_Not_Null_Or_Empty_Empty()
        {
            var myEnumerable = Array.Empty<int>();
            
            Assert.Throws<ArgumentOutOfRangeException>(() => myEnumerable.NotNullOrEmpty());
        }

        [Test]
        public void Validate_Enumerable_HasElementCountEqualTo_Ok()
        {
            var elementCount = new Random().Next(50);
            var myEnumerable = Enumerable.Range(0, elementCount).ToList();
            myEnumerable.HasElementCountEqualTo(elementCount);

            Assert.Pass();
        }

        [Test]
        public void Validate_Enumerable_HasElementCountEqualTo_Not_Ok()
        {
            var elementCount = new Random().Next(50);
            var myEnumerable = Enumerable.Range(0, elementCount + 10).ToList();

            Assert.Throws<ArgumentOutOfRangeException>(() => myEnumerable.HasElementCountEqualTo(elementCount));
        }

        [Test]
        public void Validate_Enumerable_HasElementCount_With_Check_Ok()
        {
            var elementCount = new Random().Next(50);
            var myEnumerable = Enumerable.Range(0, elementCount + 10).ToList();
            myEnumerable.HasElementCount(c => c >= elementCount);

            Assert.Pass();
        }

        [Test]
        public void Validate_Enumerable_HasElementCount_With_Check_Not_Ok()
        {
            var elementCount = new Random().Next(50);
            var myEnumerable = Enumerable.Range(0, elementCount + 10).ToList();

            Assert.Throws<ArgumentOutOfRangeException>(() => myEnumerable.HasElementCount(c => c < elementCount));
        }

        [Test]
        public void Validate_Enumerable_FormsLoop_Ok()
        {
            var elementCount = new Random().Next(50);
            var myEnumerable = Enumerable.Range(0, elementCount).Select((n, i) => (Start: i, End: n + 1)).ToList();
            myEnumerable[myEnumerable.Count - 1] = (myEnumerable[myEnumerable.Count - 1].Start, 0);
            myEnumerable.FormsLoop();

            Assert.Pass();
        }

        [Test]
        public void Validate_Enumerable_FormsLoop_Not_Ok()
        {
            var elementCount = new Random().Next(50);
            var myEnumerable = Enumerable.Range(0, elementCount).Select((n, i) => (Start: i, End: n + 1)).ToList();
            myEnumerable[myEnumerable.Count - 1] = (myEnumerable[myEnumerable.Count - 1].Start, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => myEnumerable.FormsLoop());
        }
    }
}