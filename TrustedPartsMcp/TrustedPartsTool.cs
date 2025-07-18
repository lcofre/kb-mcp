using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ModelContextProtocol.Server;

namespace TrustedPartsMcp;

[McpServerToolType]
public static class TrustedPartsTool
{
    [McpServerTool, Description("Get a list of parts from TrustedParts.com.")]
    public static async Task<string> GetParts(string query)
    {
        var httpClient = new HttpClient();
        var request = new
        {
            CompanyId = "your_company_id", // Replace with your Company ID
            ApiKey = "your_api_key",       // Replace with your API Key
            Queries = new[]
            {
                new { SearchToken = query }
            }
        };
        var json = JsonSerializer.Serialize(request);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://api.trustedparts.com/v2/search", data);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}
