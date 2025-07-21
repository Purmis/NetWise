using CatFactsApp.Models;
using Xunit;

namespace CatFactsApp.Tests.Models
{
    public class CatFactTests
    {
        [Fact]
        public void CatFact_DefaultConstructor_ShouldInitializeProperties()
        {
            // Act
            var catFact = new CatFact();

            // Assert
            Assert.Equal(string.Empty, catFact.Fact);
            Assert.Equal(0, catFact.Length);
        }

        [Fact]
        public void CatFact_SetProperties_ShouldRetainValues()
        {
            // Arrange
            var expectedFact = "Cats have 32 muscles in each ear.";
            var expectedLength = 32;

            // Act
            var catFact = new CatFact
            {
                Fact = expectedFact,
                Length = expectedLength
            };

            // Assert
            Assert.Equal(expectedFact, catFact.Fact);
            Assert.Equal(expectedLength, catFact.Length);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("A", 1)]
        [InlineData("Short fact", 10)]
        [InlineData("This is a longer cat fact that contains more information about cats and their behavior.", 85)]
        public void CatFact_WithVariousFactLengths_ShouldStoreCorrectly(string fact, int length)
        {
            // Act
            var catFact = new CatFact
            {
                Fact = fact,
                Length = length
            };

            // Assert
            Assert.Equal(fact, catFact.Fact);
            Assert.Equal(length, catFact.Length);
        }

        [Fact]
        public void CatFact_WithNullFact_ShouldAllowNullValue()
        {
            // Act
            var catFact = new CatFact
            {
                Fact = null!,
                Length = 0
            };

            // Assert
            Assert.Null(catFact.Fact);
            Assert.Equal(0, catFact.Length);
        }

        [Fact]
        public void CatFact_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var factWithSpecialChars = "Cats can make over 100 different sounds! 🐱 (Dogs can only make 10)";
            var expectedLength = 67;

            // Act
            var catFact = new CatFact
            {
                Fact = factWithSpecialChars,
                Length = expectedLength
            };

            // Assert
            Assert.Equal(factWithSpecialChars, catFact.Fact);
            Assert.Equal(expectedLength, catFact.Length);
        }

        [Fact]
        public void CatFact_WithUnicodeCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var unicodeFact = "Koty mają 32 mięśnie w każdym uchu. 🇵🇱";
            var expectedLength = 37;

            // Act
            var catFact = new CatFact
            {
                Fact = unicodeFact,
                Length = expectedLength
            };

            // Assert
            Assert.Equal(unicodeFact, catFact.Fact);
            Assert.Equal(expectedLength, catFact.Length);
        }

        [Fact]
        public void CatFact_PropertyChanges_ShouldReflectNewValues()
        {
            // Arrange
            var catFact = new CatFact
            {
                Fact = "Original fact",
                Length = 13
            };

            var newFact = "Updated cat fact";
            var newLength = 16;

            // Act
            catFact.Fact = newFact;
            catFact.Length = newLength;

            // Assert
            Assert.Equal(newFact, catFact.Fact);
            Assert.Equal(newLength, catFact.Length);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(int.MinValue)]
        public void CatFact_WithNegativeLength_ShouldAllowNegativeValues(int negativeLength)
        {
            // Act
            var catFact = new CatFact
            {
                Fact = "Test fact",
                Length = negativeLength
            };

            // Assert
            Assert.Equal(negativeLength, catFact.Length);
        }

        [Fact]
        public void CatFact_WithMaxIntLength_ShouldHandleLargeValues()
        {
            // Arrange
            var maxLength = int.MaxValue;

            // Act
            var catFact = new CatFact
            {
                Fact = "Test fact",
                Length = maxLength
            };

            // Assert
            Assert.Equal(maxLength, catFact.Length);
        }
    }
}
