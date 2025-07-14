namespace McpServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateEmptyApplicationBuilder(settings: null);

            builder.Services.AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly(typeof(Program).Assembly);

            builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();

            var app = builder.Build();

            await app.RunAsync();
        }
    }
}
