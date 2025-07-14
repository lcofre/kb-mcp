using Moq;
using McpServer.Tools;

namespace McpServer.Tests
{
    public class EmailToolTests
    {
        private readonly Mock<IElasticsearchService> _mockElasticsearchService;
        private readonly EmailTool _emailTool;

        public EmailToolTests()
        {
            _mockElasticsearchService = new Mock<IElasticsearchService>();
            _emailTool = new EmailTool(_mockElasticsearchService.Object);
        }

        [Fact]
        public async Task SearchEmails_ReturnsFormattedString_WhenEmailsAreFound()
        {
            // Arrange
            var emails = new List<Email>
            {
                new Email { Subject = "Test Subject 1", Body = "Test Body 1" },
                new Email { Subject = "Test Subject 2", Body = "Test Body 2" }
            };
            _mockElasticsearchService.Setup(s => s.SearchAsync("test")).ReturnsAsync(emails);

            // Act
            var result = await _emailTool.SearchEmails("test");

            // Assert
            var expected = "Subject: Test Subject 1\nBody: Test Body 1\n--\nSubject: Test Subject 2\nBody: Test Body 2";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task SearchEmails_ReturnsEmptyString_WhenNoEmailsFound()
        {
            // Arrange
            var emails = new List<Email>();
            _mockElasticsearchService.Setup(s => s.SearchAsync("nonexistent")).ReturnsAsync(emails);

            // Act
            var result = await _emailTool.SearchEmails("nonexistent");

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public async Task SearchEmails_HandlesSingleEmail()
        {
            // Arrange
            var emails = new List<Email>
            {
                new Email { Subject = "Single Email", Body = "Single Body" }
            };
            _mockElasticsearchService.Setup(s => s.SearchAsync("single")).ReturnsAsync(emails);

            // Act
            var result = await _emailTool.SearchEmails("single");

            // Assert
            var expected = "Subject: Single Email\nBody: Single Body";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task SearchEmails_HandlesNullSubject()
        {
            // Arrange
            var emails = new List<Email>
            {
                new Email { Subject = null, Body = "Test Body" }
            };
            _mockElasticsearchService.Setup(s => s.SearchAsync("test")).ReturnsAsync(emails);

            // Act
            var result = await _emailTool.SearchEmails("test");

            // Assert
            var expected = "Subject: \nBody: Test Body";
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task SearchEmails_HandlesNullBody()
        {
            // Arrange
            var emails = new List<Email>
            {
                new Email { Subject = "Test Subject", Body = null }
            };
            _mockElasticsearchService.Setup(s => s.SearchAsync("test")).ReturnsAsync(emails);

            // Act
            var result = await _emailTool.SearchEmails("test");

            // Assert
            var expected = "Subject: Test Subject\nBody: ";
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("search term")]
        [InlineData("complex search with multiple words")]
        public async Task SearchEmails_PassesQueryToElasticsearchService(string query)
        {
            // Arrange
            var emails = new List<Email>();
            _mockElasticsearchService.Setup(s => s.SearchAsync(query)).ReturnsAsync(emails);

            // Act
            await _emailTool.SearchEmails(query);

            // Assert
            _mockElasticsearchService.Verify(s => s.SearchAsync(query), Times.Once);
        }

        [Fact]
        public async Task SearchEmails_HandlesLargeNumberOfEmails()
        {
            // Arrange
            var emails = new List<Email>();
            for (int i = 0; i < 100; i++)
            {
                emails.Add(new Email { Subject = $"Subject {i}", Body = $"Body {i}" });
            }
            _mockElasticsearchService.Setup(s => s.SearchAsync("many")).ReturnsAsync(emails);

            // Act
            var result = await _emailTool.SearchEmails("many");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Subject 0", result);
            Assert.Contains("Subject 99", result);
            Assert.Contains("--", result); // Separator should be present
        }
    }
}
