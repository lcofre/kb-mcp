namespace EmailReader.Tests
{
    public class EmailTests
    {
        [Fact]
        public void Email_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var email = new Email();

            // Assert
            Assert.Null(email.Id);
            Assert.Null(email.From);
            Assert.Null(email.To);
            Assert.Null(email.Cc);
            Assert.Null(email.Bcc);
            Assert.Null(email.Subject);
            Assert.Null(email.Body);
            Assert.Equal(default(DateTime), email.Date);
            Assert.Null(email.Attachments);
        }

        [Fact]
        public void Email_ShouldSetAndGetProperties()
        {
            // Arrange
            var expectedId = "test-id-123";
            var expectedFrom = new[] { "sender@example.com" };
            var expectedTo = new[] { "recipient@example.com", "recipient2@example.com" };
            var expectedCc = new[] { "cc@example.com" };
            var expectedBcc = new[] { "bcc@example.com" };
            var expectedSubject = "Test Subject";
            var expectedBody = "Test email body content";
            var expectedDate = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var expectedAttachments = new[] { "document.pdf", "image.jpg" };

            // Act
            var email = new Email
            {
                Id = expectedId,
                From = expectedFrom,
                To = expectedTo,
                Cc = expectedCc,
                Bcc = expectedBcc,
                Subject = expectedSubject,
                Body = expectedBody,
                Date = expectedDate,
                Attachments = expectedAttachments
            };

            // Assert
            Assert.Equal(expectedId, email.Id);
            Assert.Equal(expectedFrom, email.From);
            Assert.Equal(expectedTo, email.To);
            Assert.Equal(expectedCc, email.Cc);
            Assert.Equal(expectedBcc, email.Bcc);
            Assert.Equal(expectedSubject, email.Subject);
            Assert.Equal(expectedBody, email.Body);
            Assert.Equal(expectedDate, email.Date);
            Assert.Equal(expectedAttachments, email.Attachments);
        }

        [Fact]
        public void Email_ShouldHandleEmptyArrays()
        {
            // Arrange & Act
            var email = new Email
            {
                From = new string[0],
                To = new string[0],
                Cc = new string[0],
                Bcc = new string[0],
                Attachments = new string[0]
            };

            // Assert
            Assert.NotNull(email.From);
            Assert.Empty(email.From);
            Assert.NotNull(email.To);
            Assert.Empty(email.To);
            Assert.NotNull(email.Cc);
            Assert.Empty(email.Cc);
            Assert.NotNull(email.Bcc);
            Assert.Empty(email.Bcc);
            Assert.NotNull(email.Attachments);
            Assert.Empty(email.Attachments);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("valid-id")]
        [InlineData("complex-id-with-special-chars@domain.com")]
        public void Email_ShouldAcceptVariousIdFormats(string id)
        {
            // Arrange & Act
            var email = new Email { Id = id };

            // Assert
            Assert.Equal(id, email.Id);
        }

        [Fact]
        public void Email_ShouldHandleUtcDateTimes()
        {
            // Arrange
            var utcDate = DateTime.UtcNow;
            var email = new Email { Date = utcDate };

            // Act & Assert
            Assert.Equal(utcDate, email.Date);
            Assert.Equal(DateTimeKind.Utc, email.Date.Kind);
        }
    }
}
