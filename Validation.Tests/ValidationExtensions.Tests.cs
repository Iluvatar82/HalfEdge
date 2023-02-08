using HalfEdge.Models;

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
            Assert.Throws<ArgumentNullException>(() => myEnumerable?.NotNullOrEmpty());
        }

        [Test]
        public void Validate_Enumerable_Not_Null_Or_Empty_Empty()
        {
            var myEnumerable = Array.Empty<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => myEnumerable.NotNullOrEmpty());
        }
    }
}