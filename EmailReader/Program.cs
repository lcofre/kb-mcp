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
            });
            var logger = loggerFactory.CreateLogger<EmailReaderService>();
            var emailReaderService = new EmailReaderService(configuration, logger);
            await emailReaderService.ReadAndIndexEmailsAsync(CancellationToken.None);
        }
    }
}
