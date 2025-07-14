namespace McpServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMcpServer()
                .WithHttpTransport()
                .WithToolsFromAssembly();

            builder.Services.AddSingleton<IElasticsearchService, ElasticsearchService>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.MapMcp("api/mcp");
            app.UseCors();

            await app.RunAsync();
        }
    }
}
