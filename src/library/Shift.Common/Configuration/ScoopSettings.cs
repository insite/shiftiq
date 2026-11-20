namespace Shift.Common
{
    public class ScoopSettings
    {
        public string BaseUrl { get; set; }
        public string CallbackPath { get; set; }
        public TokenRelaySettings Relay { get; set; }

        /// <summary>
        /// The cross-tenant read-only partner key (osp_...) for the Scoop
        /// course lookup, issued by Scoop rather than by a tenant manager.
        /// </summary>
        /// <remarks>
        /// When this is empty the lookup is skipped rather than attempted, so a
        /// deployment that never received the key degrades quietly to the
        /// English package. That is deliberate for the learner and a trap for
        /// the operator: it must be set by the same release step that sets
        /// Scoop.Relay.Secret, or nobody will notice it is missing.
        /// </remarks>
        public string ApiKey { get; set; }

        /// <summary>
        /// How long to wait for the course lookup. Zero or missing means
        /// ScoopCourseClient.DefaultTimeoutSeconds. The call happens while a
        /// learner's page is rendering, so this is a small number.
        /// </summary>
        public int LookupTimeoutSeconds { get; set; }
    }
}
