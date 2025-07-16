using MailKit.Net.Imap;

namespace EmailReader
{
    public class ImapClientFactory : IImapClientFactory
    {
        public IImapClient CreateImapClient()
        {
            return new ImapClient();
        }
    }
}
