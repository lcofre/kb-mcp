using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using McpServer.Tools;
using Nest;

namespace McpServer.Tests
{
    public class EmailToolTests
    {
        [Fact]
        public async Task SearchEmails_ReturnsFormattedString_WhenEmailsAreFound()
        {
            // Arrange
            var mockElasticsearchService = new Mock<IElasticsearchService>();
            var emails = new List<Email>
            {
                new Email { Subject = "Test Subject 1", Body = "Test Body 1" },
                new Email { Subject = "Test Subject 2", Body = "Test Body 2" }
            };
            mockElasticsearchService.Setup(s => s.SearchAsync("test")).ReturnsAsync(emails);

            var emailTool = new EmailTool(mockElasticsearchService.Object);

            // Act
            var result = await emailTool.SearchEmails("test");

            // Assert
            var expected = "Subject: Test Subject 1\nBody: Test Body 1\n--\nSubject: Test Subject 2\nBody: Test Body 2";
            Assert.Equal(expected, result);
        }
    }
}
