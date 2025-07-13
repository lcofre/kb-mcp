using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Nest;

namespace EmailReader
{
    public class EmailReaderService
    {
        private readonly IImapClient _imapClient;
        private readonly IElasticClient? _elasticClient;
        private readonly string[]? _folders;

        public EmailReaderService(IConfiguration configuration)
        {
            var host = configuration["Imap:Host"];
            var portValue = configuration["Imap:Port"];
            var port = !string.IsNullOrEmpty(portValue) ? int.Parse(portValue) : 993;
            var useSslValue = configuration["Imap:UseSsl"];
            var useSsl = !string.IsNullOrEmpty(useSslValue) ? bool.Parse(useSslValue) : true;
            var username = configuration["Imap:Username"];
            var password = configuration["Imap:Password"];
            _folders = configuration.GetSection("Imap:Folders").Get<string[]>();

            _imapClient = new ImapClient();
            if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                _imapClient.Connect(host, port, useSsl);
                _imapClient.Authenticate(username, password);
            }


            var elasticUrl = configuration["Elasticsearch:Url"];
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                var settings = new ConnectionSettings(new Uri(elasticUrl));
                _elasticClient = new ElasticClient(settings);
            }
        }

        public async Task ReadAndIndexEmailsAsync(CancellationToken cancellationToken)
        {
            if (_elasticClient == null || _imapClient == null || _folders == null)
            {
                return;
            }
            await CreateIndexTemplateAsync(cancellationToken);

            foreach (var folderName in _folders)
            {
                var folder = _imapClient.GetFolder(folderName);
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
                        Attachments = message.Attachments.Select(a => a.ContentDisposition?.FileName ?? "unknown").ToArray()
                    };

                    await _elasticClient.IndexDocumentAsync(email, cancellationToken);
                }
            }
        }

        private async Task CreateIndexTemplateAsync(CancellationToken cancellationToken)
        {
            var template = await File.ReadAllTextAsync("elasticsearch_template.json", cancellationToken);
            await _elasticClient.LowLevel.DoRequestAsync<BytesResponse>(Elasticsearch.Net.HttpMethod.PUT, "_template/emails_template", cancellationToken, PostData.String(template));
        }
    }
}
