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
using Moq;
using Xunit;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using System;

namespace EmailReader.Tests
{
    public class EmailReaderServiceTests
    {
        private readonly Mock<IImapClient> _imapClientMock;
        private readonly Mock<ElasticsearchClient> _elasticClientMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly EmailReaderService _emailReaderService;

        public EmailReaderServiceTests()
        {
            _imapClientMock = new Mock<IImapClient>();
            _elasticClientMock = new Mock<ElasticsearchClient>();
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

            _emailReaderService = new EmailReaderService(_configurationMock.Object);
        }

    }
}
