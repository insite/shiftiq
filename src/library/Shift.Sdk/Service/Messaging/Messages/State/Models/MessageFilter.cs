using System;

using Shift.Common;

namespace InSite.Domain.Messages
{
    [Serializable]
    public class MessageFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid? SenderIdentifier { get; set; }

        public Guid[] MessageIdentifiers { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }

        public string[] IncludeTypes { get; set; }
        public string[] ExcludeTypes { get; set; }
        public string Type
        {
            get => IncludeTypes != null && IncludeTypes.Length == 1 ? IncludeTypes[0] : null;
            set => IncludeTypes = value.IsEmpty() ? null : new[] { value };
        }

        public bool? HasSender { get; set; }
        public bool? IsDisabled { get; set; }

        public DateTimeRange Modified { get; set; }

        public string SenderNickname { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderType { get; set; }
        public string SystemMailbox { get; set; }

        public string TimeZone { get; set; }

        public MessageFilter()
        {
            Modified = new DateTimeRange();
            HasSender = true;
        }

        public MessageFilter Clone()
        {
            return (MessageFilter)MemberwiseClone();
        }
    }
}
