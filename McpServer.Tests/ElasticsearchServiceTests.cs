using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using System.Linq;

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

        [Fact]
        public async Task SearchAsync_ReturnsEmails_WhenSearchIsSuccessful()
        {
            // Arrange
            var mockClient = new Mock<ElasticsearchClient>();
            var emails = new List<Email>
            {
                new Email { Subject = "Test Subject 1", Body = "Test Body 1" },
                new Email { Subject = "Test Subject 2", Body = "Test Body 2" }
            };

            var mockSearchResponse = new Mock<SearchResponse<Email>>();
            mockSearchResponse.Setup(r => r.Documents).Returns(emails);

            mockClient.Setup(c => c.SearchAsync<Email>())
                .ReturnsAsync(mockSearchResponse.Object);

            var service = new ElasticsearchService(mockClient.Object, "emails");

            // Act
            var result = await service.SearchAsync("test");

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}
