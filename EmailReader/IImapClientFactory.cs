using MailKit.Net.Imap;

namespace EmailReader
{
    public interface IImapClientFactory
    {
        IImapClient CreateImapClient();
    }
}
