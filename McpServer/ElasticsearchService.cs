using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;

namespace McpServer
{
    public interface IElasticsearchService
    {
        Task<IEnumerable<Email>> SearchAsync(string query);
    }

    public class ElasticsearchService : IElasticsearchService
    {
        private readonly ElasticsearchClient _client;
        private readonly string _defaultIndex;

        public ElasticsearchService(IConfiguration configuration)
        {
            var url = configuration["Elasticsearch:Url"];
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Elasticsearch URL is not configured.");
            }
            _defaultIndex = configuration["Elasticsearch:DefaultIndex"];

            var settings = new ElasticsearchClientSettings(new Uri(url));

            _client = new ElasticsearchClient(settings);
        }

        // Add this constructor for testing purposes
        public ElasticsearchService(ElasticsearchClient client, string defaultIndex)
        {
            _client = client;
            _defaultIndex = defaultIndex;
        }

        public virtual async Task<IEnumerable<Email>> SearchAsync(string query)
        {
            var searchResponse = await _client.SearchAsync<Email>(s => s
                .Index(_defaultIndex)
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(query)
                        .Fields("subject,body")
                    )
                )
            );

            return searchResponse.Documents;
        }
    }

    public class Email
    {
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
