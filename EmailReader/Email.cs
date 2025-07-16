namespace EmailReader
{
    public class Email
    {
        public string? Id { get; set; }
        public string[]? From { get; set; }
        public string[]? To { get; set; }
        public string[]? Cc { get; set; }
        public string[]? Bcc { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime Date { get; set; }
        public string[]? Attachments { get; set; }
        public string? Inbox { get; set; }
    }
}
