using Moq;
using Microsoft.Extensions.Configuration;
using Elastic.Clients.Elasticsearch;

namespace McpServer.Tests
{
    public class ElasticsearchServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;

        public ElasticsearchServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [Fact]
        public void Constructor_ThrowsException_WhenUrlIsNotConfigured()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["Elasticsearch:Url"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<Exception>(() => new ElasticsearchService(_mockConfiguration.Object));
        }

        [Fact]
        public void Constructor_ThrowsException_WhenUrlIsEmpty()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["Elasticsearch:Url"]).Returns("");

            // Act & Assert
            Assert.Throws<Exception>(() => new ElasticsearchService(_mockConfiguration.Object));
        }

        [Fact]
        public void Constructor_CreatesService_WhenValidUrlIsProvided()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["Elasticsearch:Url"]).Returns("http://localhost:9200");
            _mockConfiguration.Setup(c => c["Elasticsearch:DefaultIndex"]).Returns("emails");

            // Act
            var service = new ElasticsearchService(_mockConfiguration.Object);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithClientAndIndex_CreatesService()
        {
            // Arrange
            var mockClient = new Mock<ElasticsearchClient>();
            var defaultIndex = "test-index";

            // Act
            var service = new ElasticsearchService(mockClient.Object, defaultIndex);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmptyList_WhenDefaultIndexIsNull()
        {
            // Arrange
            var mockClient = new Mock<ElasticsearchClient>();
            var service = new ElasticsearchService(mockClient.Object, string.Empty);

            // Act
            var result = await service.SearchAsync("test query");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsEmptyList_WhenDefaultIndexIsEmpty()
        {
            // Arrange
            var mockClient = new Mock<ElasticsearchClient>();
            var service = new ElasticsearchService(mockClient.Object, "");

            // Act
            var result = await service.SearchAsync("test query");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
