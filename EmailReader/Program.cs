using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmailReader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddFilter("Microsoft", LogLevel.Warning);
                builder.AddFilter("System", LogLevel.Warning);
                builder.AddFilter("EmailReader", LogLevel.Debug);
            });

            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Starting EmailReader service.");

            var emailReaderService = new EmailReaderService(configuration, loggerFactory.CreateLogger<EmailReaderService>());
            await emailReaderService.ReadAndIndexEmailsAsync(CancellationToken.None);

            logger.LogInformation("EmailReader service finished.");
        }
    }
}
