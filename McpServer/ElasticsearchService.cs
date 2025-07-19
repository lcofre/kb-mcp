using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;

namespace McpServer
{
    public interface IElasticsearchService
    {
        Task<IEnumerable<Email>> SearchAsync(string query);
    }

    public class ElasticsearchService : IElasticsearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly string? _defaultIndex;
        private readonly ILogger<ElasticsearchService> _logger;

        public ElasticsearchService(IConfiguration configuration, ILogger<ElasticsearchService> logger)
        {
            _logger = logger;
            var url = configuration["Elasticsearch:Url"];
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogError("Elasticsearch URL is not configured.");
                throw new Exception("Elasticsearch URL is not configured.");
            }
            _logger.LogInformation("Elasticsearch URL: {url}", url);
            _defaultIndex = configuration["Elasticsearch:DefaultIndex"];

            var settings = new ElasticsearchClientSettings(new Uri(url));

            _client = new ElasticsearchClient(settings);
        }

        // Add this constructor for testing purposes
        public ElasticsearchService(ElasticsearchClient client, string defaultIndex, ILogger<ElasticsearchService> logger)
        {
            _client = client;
            _defaultIndex = defaultIndex;
            _logger = logger;
        }

        public virtual async Task<IEnumerable<Email>> SearchAsync(string query)
        {
            if (string.IsNullOrEmpty(_defaultIndex))
            {
                return new List<Email>();
            }

            try
            {
                var searchResponse = await _client.SearchAsync<Email>(s => s
                    .Indices(_defaultIndex)
                    .Query(q => q
                        .MultiMatch(m => m
                            .Query(query)
                            .Fields("subject,body")
                        )
                    )
                );

                if (searchResponse.IsValidResponse)
                {
                    return searchResponse.Documents;
                }
                else
                {
                    _logger.LogError("Elasticsearch query failed: {reason}", searchResponse.ElasticsearchServerError);
                    return new List<Email>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search emails");
                return new List<Email>();
            }
        }
    }

}
