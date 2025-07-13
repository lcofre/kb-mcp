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

    }
}
