using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpServer.Tools
{
    [McpServerToolType]
    public class EmailTool
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ILogger<EmailTool> _logger;

        public EmailTool(IElasticsearchService elasticsearchService, ILogger<EmailTool> logger)
        {
            _elasticsearchService = elasticsearchService;
            _logger = logger;
        }

        [McpServerTool, Description("Search for emails in the support inboxes.")]
        public async Task<string> SearchEmails(
            [Description("The search query.")] string query)
        {
            _logger.LogInformation("Searching for emails with query: {query}", query);
            var emails = await _elasticsearchService.SearchAsync(query);
            _logger.LogInformation("Found {count} emails.", emails.Count());
            return string.Join("\n--\n", emails.Select(email => $"Subject: {email.Subject}\nBody: {email.Body}"));
        }
    }
}
