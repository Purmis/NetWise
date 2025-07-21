using System.Net;
using System.Text;
using System.Text.Json;
using CatFactsApp.Models;
using CatFactsApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace CatFactsApp.Tests.Services
{
    public class CatFactServiceTests
    {
        private readonly Mock<ILogger<CatFactService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly CatFactService _catFactService;

        public CatFactServiceTests()
        {
            _mockLogger = new Mock<ILogger<CatFactService>>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _catFactService = new CatFactService(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithSuccessfulResponse_ShouldReturnCatFact()
        {
            // Arrange
            var expectedCatFact = new CatFact
            {
                Fact = "Cats have 32 muscles in each ear.",
                Length = 32
            };

            var jsonResponse = JsonSerializer.Serialize(expectedCatFact);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCatFact.Fact, result.Fact);
            Assert.Equal(expectedCatFact.Length, result.Length);

            VerifyLoggerWasCalled(LogLevel.Information, "Attempting to fetch cat fact from API");
            VerifyLoggerWasCalled(LogLevel.Information, "Successfully fetched cat fact with length");
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithHttpRequestException_ShouldReturnNullAndLogError()
        {
            // Arrange
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Error, "HTTP error occurred while fetching cat fact");
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithTimeoutException_ShouldReturnNullAndLogError()
        {
            // Arrange
            var timeoutException = new TaskCanceledException("Timeout", new TimeoutException());
            
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(timeoutException);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Error, "Request timeout while fetching cat fact");
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithInvalidJson_ShouldReturnNullAndLogError()
        {
            // Arrange
            var invalidJsonResponse = "{ invalid json }";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(invalidJsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Error, "JSON deserialization error");
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithEmptyResponse_ShouldReturnNullAndLogWarning()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "Received empty response from API");
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithNotFoundStatus_ShouldReturnNullAndLogWarning()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "API request failed with status code");
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithInternalServerError_ShouldReturnNullAndLogWarning()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "API request failed with status code");
        }

        [Fact]
        public async Task GetRandomCatFactAsync_WithNullJsonResponse_ShouldReturnNullAndLogWarning()
        {
            // Arrange
            var jsonResponse = "null";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "Failed to deserialize cat fact from JSON response");
        }

        [Fact]
        public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CatFactService(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new CatFactService(_httpClient, null!));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public async Task GetRandomCatFactAsync_WithWhitespaceResponse_ShouldReturnNullAndLogWarning(string whitespaceContent)
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(whitespaceContent, Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _catFactService.GetRandomCatFactAsync();

            // Assert
            Assert.Null(result);
            VerifyLoggerWasCalled(LogLevel.Warning, "Received empty response from API");
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
                Times.AtLeastOnce);
        }
    }
}
