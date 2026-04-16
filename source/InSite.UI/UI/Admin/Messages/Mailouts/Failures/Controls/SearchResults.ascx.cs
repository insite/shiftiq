using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;

using InSite.Application.Messages.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Messages.Mailouts.Failures.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<MailoutFailureFilter>
    {
        protected override int SelectCount(MailoutFailureFilter filter)
        {
            return ServiceLocator.MessageSearch.CountMailoutFailures(filter);
        }

        protected override IListSource SelectData(MailoutFailureFilter filter)
        {
            return ServiceLocator.MessageSearch.GetMailoutFailures(filter).ToSearchResult();
        }

        protected string GetLocalTime(string name)
        {
            var dataItem = Page.GetDataItem();
            var value = (DateTimeOffset?)DataBinder.Eval(dataItem, name);
            return value.Format(User.TimeZone, true, true);
        }

        protected string GetRecipientHtml()
        {
            var dataItem = (VMailoutFailure)Page.GetDataItem();
            var html = new StringBuilder();

            var recipientName = dataItem.PersonName.IfNullOrEmpty(dataItem.RecipientName).EmptyIfNull();
            if (recipientName.IsNotEmpty() && dataItem.UserIdentifier.HasValue)
                recipientName = $"<a href='/ui/admin/contacts/people/edit?contact={dataItem.UserIdentifier}'>{recipientName}</a>";

            var recipientEmail = dataItem.UserEmail.EmptyIfNull();
            if (recipientEmail.IsNotEmpty())
                recipientEmail = $"<div><small class='text-body-secondary'>{dataItem.UserEmail}</small></div>";

            return recipientName + recipientEmail;
        }
    }
}
