namespace Shift.Common
{
    public class IntegrationSettings
    {
        public Bambora Bambora { get; set; }
        public GoogleSettings Google { get; set; }
        public Jira Jira { get; set; }
        public Mailgun Mailgun { get; set; }
        public Moodle Moodle { get; set; }
        public OAuthSecret OAuthSecret { get; set; }
        public PrometricEnvironments Prometric { get; set; }
        public RemoteShares RemoteShares { get; set; }
        public SafeExamBrowser SafeExamBrowser { get; set; }
        public ApiSettings SwiftSmsGateway { get; set; }
        public SSRS SSRS { get; set; }
    }
}
