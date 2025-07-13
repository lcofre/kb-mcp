using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;

namespace McpServer.Tools
{
    [McpServerToolType]
    public class EmailTool
    {
        private readonly ElasticsearchService _elasticsearchService;

        public EmailTool(ElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        [McpServerTool, Description("Search for emails in the support inboxes.")]
        public async Task<string> SearchEmails(
            [Description("The search query.")] string query)
        {
            var emails = await _elasticsearchService.SearchAsync(query);
            return string.Join("\n--\n", emails.Select(email => $"Subject: {email.Subject}\nBody: {email.Body}"));
        }
    }
}
