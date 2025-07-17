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
            _logger.LogInformation("Starting to read and index emails.");

            if (_elasticClient == null)
            {
                _logger.LogWarning("Elasticsearch client is not configured. Exiting.");
                return;
            }

            var imapConfigs = _configuration.GetSection("Imap").Get<ImapConfig[]>();
            if (imapConfigs == null)
            {
                _logger.LogWarning("IMAP configurations not found. Exiting.");
                return;
            }

            try
            {
                await CreateIndexTemplateAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Elasticsearch index template.");
                return;
            }

            foreach (var imapConfig in imapConfigs)
            {
                _logger.LogInformation($"Processing IMAP configuration for host: {imapConfig.Host}");

                if (string.IsNullOrEmpty(imapConfig.Host) || string.IsNullOrEmpty(imapConfig.Username) || string.IsNullOrEmpty(imapConfig.Password))
                {
                    _logger.LogWarning("IMAP configuration is missing host, username, or password. Skipping.");
                    continue;
                }

                try
                {
                    using var imapClient = _imapClientFactory.CreateImapClient();

                    _logger.LogInformation($"Connecting to IMAP host: {imapConfig.Host}");
                    await imapClient.ConnectAsync(imapConfig.Host, imapConfig.Port, imapConfig.UseSsl, cancellationToken);
                    _logger.LogInformation("IMAP client connected.");

                    _logger.LogInformation($"Authenticating with username: {imapConfig.Username}");
                    await imapClient.AuthenticateAsync(imapConfig.Username, imapConfig.Password, cancellationToken);
                    _logger.LogInformation("IMAP client authenticated.");

                    foreach (var folderName in imapConfig.Folders)
                    {
                        _logger.LogInformation($"Processing folder: {folderName}");
                        var folderSplit = folderName.Split('/');
                        var folder = await imapClient.GetFolderAsync(folderSplit[0], cancellationToken);
                        foreach (var subfolder in folderSplit.Skip(1))
                        {
                            folder = folder.GetSubfolder(subfolder);
                        }

                        if (folder == null)
                        {
                            _logger.LogWarning($"Folder {folderName} not found.");
                            continue;
                        }

                        await folder.OpenAsync(FolderAccess.ReadOnly, cancellationToken);
                        _logger.LogInformation($"Folder {folderName} opened. Message count: {folder.Count}");

                        var uids = await folder.SearchAsync(SearchQuery.All, cancellationToken);
                        _logger.LogInformation($"Found {uids.Count} messages in {folderName}.");

                        foreach (var uid in uids)
                        {
                            try
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

                                _logger.LogInformation($"Indexing email with subject: {email.Subject}");
                                await _elasticClient.IndexAsync(email, "emails", email.Id, cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error processing message with UID: {uid}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while processing IMAP configuration for host: {imapConfig.Host}");
                }
            }

            _logger.LogInformation("Finished reading and indexing emails.");
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
