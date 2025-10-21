using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.UI.Admin.Messages.Messages.Utilities
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class ContentSession
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public Guid SessionIdentifier { get; private set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserIdentifier { get; private set; }

        [JsonProperty(PropertyName = "messageId")]
        public Guid MessageIdentifier { get; private set; }

        [JsonProperty(PropertyName = "name")]
        public string DocumentName { get; set; }

        [JsonProperty(PropertyName = "style")]
        public string DocumentStyle { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string DocumentType { get; set; }

        [JsonProperty(PropertyName = "tenant")]
        public string OrganizationName { get; set; }

        [JsonProperty(PropertyName = "markdown")]
        public MultilingualString MarkdownText { get; set; }

        #endregion

        #region Construction

        [JsonConstructor]
        private ContentSession()
        {

        }

        #endregion

        #region Public methods

        public static ContentSession Create(Guid sessionId, Guid userId, Guid messageId, string organization)
        {
            var message = ServiceLocator.MessageSearch.GetMessage(messageId);
            if (message == null)
                return null;

            var content = ServiceLocator.ContentSearch.GetBlock(messageId);

            return new ContentSession
            {
                SessionIdentifier = sessionId,
                UserIdentifier = userId,
                MessageIdentifier = messageId,
                OrganizationName = organization,
                DocumentName = message.MessageName,
                DocumentStyle = "Segoe",
                MarkdownText = content.Body.Text
            };
        }

        #endregion
    }
}