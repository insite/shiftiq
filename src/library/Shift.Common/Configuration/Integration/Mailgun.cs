namespace Shift.Common
{
    public class Mailgun
    {
        public string ApiUrl { get; set; }
        public string WebhookSigningKey { get; set; }
        public MailgunDomain[] Domains { get; set; }
    }
}
