using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<MessageFilter>
    {
        protected override int SelectCount(MessageFilter filter)
        {
            return ServiceLocator.MessageSearch.CountVMessages(filter);
        }

        protected override IListSource SelectData(MessageFilter filter)
        {
            return ServiceLocator.MessageSearch.GetVMessages(filter).ToSearchResult();
        }

        public class ExportDataItem
        {
            public string ContentHtml { get; set; }
            public string ContentText { get; set; }
            public string MessageAttachments { get; set; }
            public string MessageName { get; set; }
            public string MessageTitle { get; set; }
            public string MessageType { get; set; }

            public string SenderEmail { get; set; }
            public string SenderName { get; set; }
            public string SenderNickname { get; set; }

            public string SystemMailbox { get; set; }
            public string OrganizationCode { get; set; }

            public bool IsDisabled { get; set; }
            public bool SenderEnabled { get; set; }

            public int LinkCount { get; set; }
            public int MailoutCount { get; set; }
            public int SubscriberGroupCount { get; set; }
            public int SubscriberMembershipCount { get; set; }
            public int SubscriberUserCount { get; set; }

            public DateTimeOffset LastChangeTime { get; set; }
            public string LastChangeType { get; set; }
            public Guid LastChangeUser { get; set; }
            public string LastChangeUserName { get; set; }
        }

        public override IListSource GetExportData(MessageFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<VMessage>().Select(x => new ExportDataItem
            {
                ContentHtml = x.ContentHtml,
                ContentText = x.ContentText,
                SubscriberGroupCount = x.SubscriberGroupCount ?? 0,
                IsDisabled = x.IsDisabled,
                SenderEnabled = x.SenderEnabled ?? false,
                LinkCount = x.LinkCount ?? 0,
                MailoutCount = x.MailoutCount ?? 0,
                MessageName = x.MessageName ?? "",
                MessageTitle = x.MessageTitle?? "",
                MessageAttachments = x.MessageAttachments ?? null,
                MessageType = x.MessageType ?? "",
                SenderEmail = x.SenderEmail ?? "",
                SenderName = x.SenderName ?? "",
                SenderNickname = x.SenderNickname ?? "",
                SubscriberMembershipCount = x.SubscriberMembershipCount ?? 0,
                SubscriberUserCount = x.SubscriberUserCount ?? 0,
                OrganizationCode = x.OrganizationCode ?? "",
                SystemMailbox = x.SystemMailbox ?? "",
                LastChangeTime = x.LastChangeTime,
                LastChangeType = x.LastChangeType,
                LastChangeUser = x.LastChangeUser,
                LastChangeUserName = x.LastChangeUserName
            }).ToList().ToSearchResult();
        }

        protected string GetMessageSubjectHtml(object o)
        {
            var message = (VMessage)o;
            var html = new StringBuilder();

            if (!StringHelper.Equals(message.OrganizationCode, Organization.OrganizationCode))
                html.Append($"<div class='float-end'><span class='badge bg-warning'>{message.OrganizationCode}</span></div>");

            html.Append("<div>");
            if (StringHelper.Equals(message.OrganizationCode, Organization.OrganizationCode))
                html.Append($"<a href='/ui/admin/messages/outline?message={message.MessageIdentifier}'>{message.MessageTitle}</a>");
            else
                html.Append($"<a target='_blank' href='{ServiceLocator.Urls.GetApplicationUrl(message.OrganizationCode)}/ui/admin/messages/outline?message={message.MessageIdentifier}'>{message.MessageTitle}<i class='far fa-external-link-alt ms-2'></i></a>");
            html.Append("</div>");
            if (!StringHelper.Equals(message.MessageName, message.MessageTitle))
                html.Append($"<div class='form-text'>{message.MessageName}</div>");

            return html.ToString();
        }

        protected string GetMessageLastChange(object o)
        {
            var message = (VMessage)o;

            return UserSearch.GetTimestampHtml(
                    message.LastChangeType, message.LastChangeTime, message.LastChangeUserName, Filter.TimeZone);
        }
    }
}