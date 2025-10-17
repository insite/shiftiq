namespace InSite.Domain.Integrations.Engine
{
    public class WebhookModel
    {
        public System.Guid HookIdentifier { get; set; }
        public System.Guid OrganizationIdentifier { get; set; }
        public bool HookEnabled { get; set; }
        public bool LazyHook { get; set; }
        public string ApiUrl { get; set; }
        public string ApiAuthorizationHeaderValue { get; set; }
        public string ApiAuthorizationScheme { get; set; }
        public string ObjectType { get; set; }
        public string ObjectChange { get; set; }
    }
}
