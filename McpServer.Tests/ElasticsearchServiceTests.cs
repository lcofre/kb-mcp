using Moq;
using Microsoft.Extensions.Configuration;

namespace McpServer.Tests
{
    public class ElasticsearchServiceTests
    {
        [Fact]
        public void Constructor_ThrowsException_WhenUrlIsNotConfigured()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["Elasticsearch:Url"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<Exception>(() => new ElasticsearchService(configuration.Object));
        }

    }
}
