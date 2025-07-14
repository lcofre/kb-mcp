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

namespace EmailReader.Tests
{
    public class EmailReaderServiceTests
    {
        [Fact]
        public void DummyTest()
        {
            Assert.True(true);
        }
    }
}
