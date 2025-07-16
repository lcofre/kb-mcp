namespace EmailReader
{
    public class ImapConfig
    {
        public string Host { get; set; }
        public int Port { get; set; } = 993;
        public bool UseSsl { get; set; } = true;
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] Folders { get; set; }
    }
}
