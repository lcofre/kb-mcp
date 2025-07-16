using Elastic.Clients.Elasticsearch;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmailReader
{
    public class EmailReaderService
    {
        private readonly ElasticsearchClient? _elasticClient;
        private readonly ILogger<EmailReaderService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IImapClientFactory _imapClientFactory;

        public EmailReaderService(IConfiguration configuration, ILogger<EmailReaderService> logger, IImapClientFactory imapClientFactory = null)
        {
            _logger = logger;
            _configuration = configuration;
            _imapClientFactory = imapClientFactory ?? new ImapClientFactory();

            var elasticUrl = configuration["Elasticsearch:Url"];
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                var settings = new ElasticsearchClientSettings(new Uri(elasticUrl));
                _elasticClient = new ElasticsearchClient(settings);
            }
        }

        public async Task ReadAndIndexEmailsAsync(CancellationToken cancellationToken)
        {
            if (_elasticClient == null)
            {
                return;
            }
            var imapConfigs = _configuration.GetSection("Imap").Get<ImapConfig[]>();
            if (imapConfigs == null)
            {
                return;
            }

            await CreateIndexTemplateAsync(cancellationToken);

            foreach (var imapConfig in imapConfigs)
            {
                using var imapClient = _imapClientFactory.CreateImapClient();
                if (string.IsNullOrEmpty(imapConfig.Host) || string.IsNullOrEmpty(imapConfig.Username) || string.IsNullOrEmpty(imapConfig.Password))
                {
                    continue;
                }

                await imapClient.ConnectAsync(imapConfig.Host, imapConfig.Port, imapConfig.UseSsl, cancellationToken);
                await imapClient.AuthenticateAsync(imapConfig.Username, imapConfig.Password, cancellationToken);

                foreach (var folderName in imapConfig.Folders)
                {
                    var folder = await imapClient.GetFolderAsync(folderName, cancellationToken);
                    if (folder == null)
                    {
                        _logger.LogWarning($"Folder {folderName} not found.");
                        continue;
                    }
                    await folder.OpenAsync(FolderAccess.ReadOnly, cancellationToken);

                    var uids = await folder.SearchAsync(SearchQuery.All, cancellationToken);
                    foreach (var uid in uids)
                    {
                        var message = await folder.GetMessageAsync(uid, cancellationToken);
                        var email = new Email
                        {
                            Id = message.MessageId,
                            From = message.From.Mailboxes.Select(m => m.Address).ToArray(),
                            To = message.To.Mailboxes.Select(m => m.Address).ToArray(),
                            Cc = message.Cc.Mailboxes.Select(m => m.Address).ToArray(),
                            Bcc = message.Bcc.Mailboxes.Select(m => m.Address).ToArray(),
                            Subject = message.Subject,
                            Body = message.TextBody,
                            Date = message.Date.UtcDateTime,
                            Attachments = message.Attachments.Select(a => a.ContentDisposition?.FileName ?? "unknown").ToArray(),
                            Inbox = imapConfig.Username
                        };

                        await _elasticClient.IndexAsync(email, "emails", email.Id, cancellationToken);
                    }
                }
            }
        }

        private async Task CreateIndexTemplateAsync(CancellationToken cancellationToken)
        {
            var template = await File.ReadAllTextAsync("elasticsearch_template.json", cancellationToken);
            // This is a low-level request, so we need to create a StringRequestContent object.
            // There is no direct equivalent of DoRequestAsync in the new client.
            // We will need to use the PutTemplateAsync method.
            // However, the template is in a JSON file, so we need to deserialize it first.
            // It is easier to just use the low-level client for this.
            // The new client does not expose a low-level client directly.
            // We will have to use the PutIndexTemplateAsync method.
            // This method takes a PutIndexTemplateRequest object.
            // This object can be created from a JSON string.
            // Let's read the file and create the request.
            var request = new Elastic.Clients.Elasticsearch.IndexManagement.PutIndexTemplateRequest("emails_template")
            {
                IndexPatterns = new[] { "emails-*" },
                Template = new Elastic.Clients.Elasticsearch.IndexManagement.IndexTemplateMapping
                {
                    Mappings = new Elastic.Clients.Elasticsearch.Mapping.TypeMapping
                    {
                        Properties = new Elastic.Clients.Elasticsearch.Mapping.Properties
                        {
                            { "id", new Elastic.Clients.Elasticsearch.Mapping.KeywordProperty() },
                            { "from", new Elastic.Clients.Elasticsearch.Mapping.KeywordProperty() },
                            { "to", new Elastic.Clients.Elasticsearch.Mapping.KeywordProperty() },
                            { "cc", new Elastic.Clients.Elasticsearch.Mapping.KeywordProperty() },
                            { "bcc", new Elastic.Clients.Elasticsearch.Mapping.KeywordProperty() },
                            { "subject", new Elastic.Clients.Elasticsearch.Mapping.TextProperty() },
                            { "body", new Elastic.Clients.Elasticsearch.Mapping.TextProperty() },
                            { "date", new Elastic.Clients.Elasticsearch.Mapping.DateProperty() },
                            { "attachments", new Elastic.Clients.Elasticsearch.Mapping.KeywordProperty() }
                        }
                    }
                }
            };
            await _elasticClient.Indices.PutIndexTemplateAsync(request, cancellationToken);
        }
    }
}
