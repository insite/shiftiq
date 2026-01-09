namespace Shift.Common
{
    public class SmtpServer
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public int Timeout { get; set; }
        public bool Pipelining { get; set; }
        public string AuthMethods { get; set; }
        public string AuthOptions { get; set; }
        public object AccountDomain { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string SslMode { get; set; }
        public string SslProtocol { get; set; }
        public int Priority { get; set; }
        public bool AllowRefusedRecipients { get; set; }
        public bool IgnoreLoginFailure { get; set; }
        public string SmtpOptions { get; set; }
        public bool AuthPopBeforeSmtp { get; set; }
        public int MaxConnectionCount { get; set; }
        public int MaxSendPerSessionCount { get; set; }
        public int PauseInterval { get; set; }
    }
}
