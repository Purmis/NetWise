using CatFactsApp.Models;
using CatFactsApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFactsApp.Tests.Services
{
    public class FileServiceTests : IDisposable
    {
        private readonly Mock<ILogger<FileService>> _mockLogger;
        private readonly FileService _fileService;
        private readonly string _testFilePath;

        public FileServiceTests()
        {
            _mockLogger = new Mock<ILogger<FileService>>();
            _fileService = new FileService(_mockLogger.Object);
            _testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "cat_facts.txt");
        }

        public void Dispose()
        {
            // Cleanup test file if it exists
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        [Fact]
        public async Task SaveCatFactToFileAsync_WithValidCatFact_ShouldReturnTrue()
        {
            // Arrange
            var catFact = new CatFact
            {
                Fact = "Cats have 32 muscles in each ear.",
                Length = 32
            };

            // Act
            var result = await _fileService.SaveCatFactToFileAsync(catFact);

            // Assert
            Assert.True(result);
            Assert.True(File.Exists(_testFilePath));
            
            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            Assert.Contains(catFact.Fact, fileContent);
            Assert.Contains($"Długość: {catFact.Length}", fileContent);
        }

        [Fact]
        public async Task SaveCatFactToFileAsync_WithNullCatFact_ShouldReturnFalse()
        {
            // Act
            var result = await _fileService.SaveCatFactToFileAsync(null!);

            // Assert
            Assert.False(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "Attempted to save null cat fact");
        }

        [Fact]
        public async Task SaveCatFactToFileAsync_WithEmptyFact_ShouldReturnFalse()
        {
            // Arrange
            var catFact = new CatFact
            {
                Fact = "",
                Length = 0
            };

            // Act
            var result = await _fileService.SaveCatFactToFileAsync(catFact);

            // Assert
            Assert.False(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "Attempted to save cat fact with empty or null fact text");
        }

        [Fact]
        public async Task SaveCatFactToFileAsync_WithWhitespaceFact_ShouldReturnFalse()
        {
            // Arrange
            var catFact = new CatFact
            {
                Fact = "   ",
                Length = 3
            };

            // Act
            var result = await _fileService.SaveCatFactToFileAsync(catFact);

            // Assert
            Assert.False(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "Attempted to save cat fact with empty or null fact text");
        }

        [Fact]
        public async Task SaveCatFactToFileAsync_MultipleFacts_ShouldAppendToFile()
        {
            // Arrange
            var catFact1 = new CatFact { Fact = "First cat fact", Length = 15 };
            var catFact2 = new CatFact { Fact = "Second cat fact", Length = 16 };

            // Act
            var result1 = await _fileService.SaveCatFactToFileAsync(catFact1);
            var result2 = await _fileService.SaveCatFactToFileAsync(catFact2);

            // Assert
            Assert.True(result1);
            Assert.True(result2);

            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            Assert.Contains(catFact1.Fact, fileContent);
            Assert.Contains(catFact2.Fact, fileContent);
            
            // Should have 2 lines (plus empty lines)
            var lines = fileContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(2, lines.Length);
        }

        [Fact]
        public async Task SaveCatFactToFileAsync_ShouldIncludeTimestamp()
        {
            // Arrange
            var catFact = new CatFact { Fact = "Test fact", Length = 9 };
            var beforeSave = DateTime.Now.AddSeconds(-1);

            // Act
            var result = await _fileService.SaveCatFactToFileAsync(catFact);
            var afterSave = DateTime.Now.AddSeconds(1);

            // Assert
            Assert.True(result);
            
            var fileContent = await File.ReadAllTextAsync(_testFilePath);
            
            // Extract timestamp from file content
            var timestampStart = fileContent.IndexOf('[') + 1;
            var timestampEnd = fileContent.IndexOf(']');
            var timestampString = fileContent.Substring(timestampStart, timestampEnd - timestampStart);
            
            Assert.True(DateTime.TryParse(timestampString, out var timestamp));
            Assert.True(timestamp >= beforeSave && timestamp <= afterSave);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FileService(null!));
        }

        private void VerifyLoggerWasCalled(LogLevel logLevel, string message)
        {
            _mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
