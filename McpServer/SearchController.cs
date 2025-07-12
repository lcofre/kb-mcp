using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace McpServer
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ElasticsearchService _elasticsearchService;

        public SearchController(ElasticsearchService elasticsearchService)
        {
            _elasticsearchService = elasticsearchService;
        }

        [HttpGet]
        public async Task<IEnumerable<Email>> Get(string query)
        {
            return await _elasticsearchService.SearchAsync(query);
        }
    }
}
