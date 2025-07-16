using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmailReader.Tests
{
    public class EmailReaderServiceTests
    {
        private readonly Mock<ILogger<EmailReaderService>> _mockLogger;

        public EmailReaderServiceTests()
        {
            _mockLogger = new Mock<ILogger<EmailReaderService>>();
        }

        [Fact]
        public void Constructor_ShouldInitializeWithMinimalConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            // Act & Assert - Should not throw
            var service = new EmailReaderService(configuration, _mockLogger.Object);
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_ShouldHandlePartialImapConfiguration()
        {
            // Arrange - Missing username/password so no connection attempt
            var configData = new Dictionary<string, string?>
            {
                ["Imap:Host"] = "imap.example.com",
                ["Imap:Port"] = "993",
                ["Imap:UseSsl"] = "true"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            // Act & Assert - Should not throw
            var service = new EmailReaderService(configuration, _mockLogger.Object);
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData("993")]
        [InlineData("143")]
        [InlineData("")]
        public void Constructor_ShouldParsePortCorrectly(string portValue)
        {
            // Arrange - No credentials so no connection attempt
            var configData = new Dictionary<string, string?>
            {
                ["Imap:Host"] = "test.example.com",
                ["Imap:Port"] = portValue
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            // Act & Assert - Should not throw
            var service = new EmailReaderService(configuration, _mockLogger.Object);
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("")]
        public void Constructor_ShouldParseSslCorrectly(string sslValue)
        {
            // Arrange - No credentials so no connection attempt
            var configData = new Dictionary<string, string?>
            {
                ["Imap:Host"] = "test.example.com",
                ["Imap:UseSsl"] = sslValue
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            // Act & Assert - Should not throw
            var service = new EmailReaderService(configuration, _mockLogger.Object);
            Assert.NotNull(service);
        }

        [Fact]
        public async Task ReadAndIndexEmailsAsync_ShouldReturnEarly_WhenElasticClientIsNull()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>())
                .Build();

            var service = new EmailReaderService(configuration, _mockLogger.Object);

            // Act & Assert - Should complete without error
            await service.ReadAndIndexEmailsAsync(CancellationToken.None);
        }

        [Fact]
        public async Task ReadAndIndexEmailsAsync_ShouldReturnEarly_WhenFoldersIsNull()
        {
            // Arrange
            var configData = new Dictionary<string, string?>
            {
                ["Elasticsearch:Url"] = "http://localhost:9200"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            var service = new EmailReaderService(configuration, _mockLogger.Object);

            // Act & Assert - Should complete without error
            await service.ReadAndIndexEmailsAsync(CancellationToken.None);
        }

        [Fact]
        public void Constructor_ShouldHandleElasticsearchConfiguration()
        {
            // Arrange
            var configData = new Dictionary<string, string?>
            {
                ["Elasticsearch:Url"] = "http://localhost:9200"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            // Act & Assert - Should not throw
            var service = new EmailReaderService(configuration, _mockLogger.Object);
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_ShouldHandleMissingConfiguration()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act & Assert - Should not throw
            var service = new EmailReaderService(configuration, _mockLogger.Object);
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_ShouldHandleFoldersConfiguration()
        {
            // Arrange
            var configData = new Dictionary<string, string?>
            {
                ["Imap:Folders:0"] = "INBOX",
                ["Imap:Folders:1"] = "Sent",
                ["Imap:Folders:2"] = "INBOX/Subfolder"
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            // Act & Assert - Should not throw
            var service = new EmailReaderService(configuration, _mockLogger.Object);
            Assert.NotNull(service);
        }

        [Fact]
        public async Task ReadAndIndexEmailsAsync_ShouldProcessMultipleAccounts()
        {
            // Arrange
            var configData = new Dictionary<string, string?>
            {
                { "Imap:0:Host", "host1" },
                { "Imap:0:Username", "user1" },
                { "Imap:0:Password", "pass1" },
                { "Imap:0:Folders:0", "Inbox" },
                { "Imap:1:Host", "host2" },
                { "Imap:1:Username", "user2" },
                { "Imap:1:Password", "pass2" },
                { "Imap:1:Folders:0", "Sent" },
                { "Elasticsearch:Url", "http://localhost:9200" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

            var mockImapClient = new Mock<MailKit.Net.Imap.IImapClient>();
            mockImapClient.Setup(x => x.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mockImapClient.Setup(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            mockImapClient.Setup(x => x.GetFolderAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<MailKit.IMailFolder>(null));

            var mockImapClientFactory = new Mock<IImapClientFactory>();
            mockImapClientFactory.Setup(x => x.CreateImapClient()).Returns(mockImapClient.Object);

            var service = new EmailReaderService(configuration, _mockLogger.Object, mockImapClientFactory.Object);

            // Act
            await service.ReadAndIndexEmailsAsync(CancellationToken.None);

            // Assert
            mockImapClientFactory.Verify(x => x.CreateImapClient(), Times.Exactly(2));
        }
    }
}
