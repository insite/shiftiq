using System;

using Shift.Common;

using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Application.Messages.Read
{
    public class QMessage
    {
        public Guid MessageIdentifier { get; set; }
        public Guid SenderIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public bool IsDisabled { get; set; }
        public bool AutoBccSubscribers { get; set; }

        public string ContentHtml { get; set; }
        public string ContentText { get; set; }
        public string MessageAttachments { get; set; }
        public string MessageName { get; set; }
        public string MessageTitle { get; set; }
        public string MessageType { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public Guid LastChangeUser { get; set; }
    }

    public class MessageInfo
    {
        public VMessage vMessage { get; set; }
        public QMessage qMessage { get; set; }
        public string LastChange { get; set; }
    }

    public class VMessage
    {
        public Guid LastChangeUser { get; set; }
        public Guid MessageIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid SenderIdentifier { get; set; }
        public Guid? SurveyFormIdentifier { get; set; }

        public string ContentHtml { get; set; }
        public string ContentText { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUserName { get; set; }
        public string MessageAttachments { get; set; }
        public string MessageName { get; set; }
        public string MessageTitle { get; set; }
        public string MessageType { get; set; }
        public string OrganizationCode { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SenderNickname { get; set; }
        public string SenderType { get; set; }
        public string SystemMailbox { get; set; }

        public bool AutoBccSubscribers { get; set; }
        public bool IsDisabled { get; set; }
        public bool? SenderEnabled { get; set; }

        public int? LinkCount { get; set; }
        public int? MailoutCount { get; set; }
        public int? SubscriberGroupCount { get; set; }
        public int? SubscriberMembershipCount { get; set; }
        public int? SubscriberUserCount { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }

        public string SubscriberGroupCountText
        {
            get
            {
                if ((SubscriberGroupCount ?? 0) > 0)
                    return ShiftHumanizer.ToQuantity(SubscriberGroupCount.Value, "group");
                return string.Empty;
            }
        }

        public string SubscriberUserCountText
        {
            get
            {
                if ((SubscriberUserCount ?? 0) > 0)
                    return ShiftHumanizer.ToQuantity(SubscriberUserCount.Value, "user");
                return string.Empty;
            }
        }

        public string SubscriberMembershipCountText
        {
            get
            {
                if ((SubscriberMembershipCount ?? 0) > 0)
                    return ShiftHumanizer.ToQuantity(SubscriberMembershipCount.Value, "group member");
                return string.Empty;
            }
        }

        public VMessage Clone()
        {
            var clone = new VMessage();

            this.ShallowCopyTo(clone);

            return clone;
        }
    }

    public class SearchVMessage
    {
        public Guid MessageIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string MessageName { get; set; }
        public string ContentSubject { get; set; }

        public int RecipientCount { get; set; }
        public int MailoutCount { get; set; }
    }
}
