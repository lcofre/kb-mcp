using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailReader;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Elasticsearch.Net;
using Moq;
using Nest;
using Xunit;

namespace EmailReader.Tests
{
    public class EmailReaderServiceTests
    {
        private readonly Mock<IImapClient> _imapClientMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly EmailReaderService _emailReaderService;

        public EmailReaderServiceTests()
        {
            _imapClientMock = new Mock<IImapClient>();
            _elasticClientMock = new Mock<IElasticClient>();
            _configurationMock = new Mock<IConfiguration>();

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value).Returns("Inbox");
            var configurationSectionArrayMock = new Mock<IConfigurationSection>();
            configurationSectionArrayMock.Setup(x => x.GetChildren()).Returns(new[] { configurationSectionMock.Object });

            _configurationMock.Setup(x => x.GetSection("Imap:Folders")).Returns(configurationSectionArrayMock.Object);
            _configurationMock.Setup(x => x["Imap:Host"]).Returns("localhost");
            _configurationMock.Setup(x => x["Imap:Port"]).Returns("993");
            _configurationMock.Setup(x => x["Imap:UseSsl"]).Returns("true");
            _configurationMock.Setup(x => x["Imap:Username"]).Returns("user");
            _configurationMock.Setup(x => x["Imap:Password"]).Returns("password");
            _configurationMock.Setup(x => x["Elasticsearch:Url"]).Returns("http://localhost:9200");

            var lowLevelClientMock = new Mock<IElasticLowLevelClient>();
            _elasticClientMock.Setup(x => x.LowLevel).Returns(lowLevelClientMock.Object);

            _emailReaderService = new EmailReaderService(_configurationMock.Object);
        }

        [Fact]
        public async Task ReadAndIndexEmailsAsync_ShouldIndexEmails()
        {
            // Arrange
            var folderMock = new Mock<IMailFolder>();
            _imapClientMock.Setup(x => x.GetFolder(It.IsAny<string>())).Returns(folderMock.Object);
            folderMock.Setup(x => x.SearchAsync(It.IsAny<SearchQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { new UniqueId(1) });
            var message = new MimeMessage();
            message.MessageId = "test-id";
            message.From.Add(new MailboxAddress("from", "from@test.com"));
            message.To.Add(new MailboxAddress("to", "to@test.com"));
            message.Subject = "test subject";
            message.Body = new TextPart("plain") { Text = "test body" };
            message.Date = DateTimeOffset.UtcNow;
            folderMock.Setup(x => x.GetMessageAsync(It.IsAny<UniqueId>(), It.IsAny<CancellationToken>(), null))
                .ReturnsAsync(message);

            // Act
            await _emailReaderService.ReadAndIndexEmailsAsync(CancellationToken.None);

            // Assert
            _elasticClientMock.Verify(x => x.IndexDocumentAsync(It.Is<Email>(e => e.Id == "test-id"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
