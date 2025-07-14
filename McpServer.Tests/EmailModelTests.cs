namespace McpServer.Tests
{
    public class EmailModelTests
    {
        [Fact]
        public void Email_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var email = new Email();

            // Assert
            Assert.Null(email.Subject);
            Assert.Null(email.Body);
        }

        [Fact]
        public void Email_ShouldSetAndGetProperties()
        {
            // Arrange
            var expectedSubject = "Test Email Subject";
            var expectedBody = "This is the email body content with multiple lines.\nSecond line of content.";

            // Act
            var email = new Email
            {
                Subject = expectedSubject,
                Body = expectedBody
            };

            // Assert
            Assert.Equal(expectedSubject, email.Subject);
            Assert.Equal(expectedBody, email.Body);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("Short subject")]
        [InlineData("Very long subject line that contains multiple words and could potentially be very lengthy in real-world scenarios")]
        public void Email_ShouldAcceptVariousSubjectFormats(string subject)
        {
            // Arrange & Act
            var email = new Email { Subject = subject };

            // Assert
            Assert.Equal(subject, email.Subject);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Simple body")]
        [InlineData("Body with\nmultiple\nlines")]
        [InlineData("Body with special characters: !@#$%^&*()")]
        public void Email_ShouldAcceptVariousBodyFormats(string body)
        {
            // Arrange & Act
            var email = new Email { Body = body };

            // Assert
            Assert.Equal(body, email.Body);
        }

        [Fact]
        public void Email_ShouldHandleNullValues()
        {
            // Arrange & Act
            var email = new Email
            {
                Subject = null,
                Body = null
            };

            // Assert
            Assert.Null(email.Subject);
            Assert.Null(email.Body);
        }
    }
}
