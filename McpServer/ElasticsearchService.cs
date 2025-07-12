using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;

namespace McpServer
{
    public class ElasticsearchService
    {
        private readonly ElasticClient _client;

        public ElasticsearchService(IConfiguration configuration)
        {
            var url = configuration["Elasticsearch:Url"];
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Elasticsearch URL is not configured.");
            }
            var defaultIndex = configuration["Elasticsearch:DefaultIndex"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);

            _client = new ElasticClient(settings);
        }

        public async Task<IEnumerable<Email>> SearchAsync(string query)
        {
            var searchResponse = await _client.SearchAsync<Email>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Query(query)
                        .Fields(f => f
                            .Field(p => p.Subject)
                            .Field(p => p.Body)
                        )
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
