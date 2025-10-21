using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Messages.Emails.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<EmailFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public string SenderEmail { get; set; }
            public string SenderName { get; set; }
            public string EmailSubject { get; set; }
            public string EmailTo { get; set; }
        }

        private class SearchDataItem
        {
            public string SenderEmail { get; set; }
            public string SenderName { get; set; }
            public string EmailSubject { get; set; }
            public string EmailTo { get; set; }
            public string UserTo { get; set; }
            public string StatusMessage { get; set; }
            public string OrganizationName { get; set; }
            public DateTimeOffset? DeliveryTime { get; set; }
            public bool DeliverySuccessful { get; set; }
            public bool ReDeliverySuccessful { get; set; }
            public string UserFullName { get; set; }
            public Guid? MessageIdentifier { get; set; }
            public Guid? SenderIdentifier { get; set; }
            public Guid? UserIdentifier { get; set; }
            public Guid? UserToIdentifier { get; set; }
            public Guid? EmailIdentifier { get; set; }
        }

        #endregion

        public delegate void ErrorHandler(string message);
        public new event ErrorHandler OnError;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        #region Methods (data binding)

        protected override int SelectCount(EmailFilter filter)
        {
            return SelectData(filter).GetList().Count;
        }

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Resend":
                    
                    var mailoutIdentifier = Guid.Parse(e.CommandArgument as string);
                    
                    var mailout = TEmailSearch.Get(mailoutIdentifier);
                    if (mailout == null)
                        return;

                    var email = EmailOutbox.ConvertMailoutToEmail(mailout);
                    if (email == null)
                        return;

                    if (email.SenderType == "DirectAccess")
                    {
                        OnError?.Invoke("This message is sent through Direct Access. Please resend the notification from the Exam Event.");
                        return;
                    }
                    else if (email.SenderType != "Mailgun")
                    {
                        OnError?.Invoke($"The email cannot be resent through {email.SenderType}");
                        return;
                    }

                    ServiceLocator.AlertMailer.Send(email);
                    
                    RefreshGrid();
                    
                    break;
            }
        }

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(EmailFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<SearchDataItem>().Select(x => new ExportDataItem
            {
                SenderEmail = x.SenderEmail,
                SenderName = x.SenderName,
                EmailSubject = x.EmailSubject,
                EmailTo = x.EmailTo
            }).ToList().ToSearchResult();
        }

        protected override IListSource SelectData(EmailFilter filter)
        {
            var partialResults = EmailSearch.Select(filter)
                .Select(x => new SearchDataItem
                {
                    SenderEmail = x.SenderEmail,
                    SenderName = x.SenderName,
                    EmailSubject = x.ContentSubject,
                    EmailTo = x.RecipientListTo,
                    DeliveryTime = x.MailoutScheduled,
                    DeliverySuccessful = x.MailoutStatus == "Completed" || x.MailoutStatus == "Succeeded",
                    StatusMessage = x.MailoutStatus,
                    UserFullName = x.UserName,
                    OrganizationName = x.OrganizationName,
                    MessageIdentifier = x.MessageIdentifier,
                    SenderIdentifier = x.SenderIdentifier,
                    UserIdentifier = x.UserIdentifier,
                    EmailIdentifier = x.MailoutIdentifier
                }).ToList();

            if (filter.Paging != null)
                return BindRecipients(partialResults, filter).ToSearchResult();

            return partialResults.ToSearchResult();
        }

        private List<SearchDataItem> BindRecipients(List<SearchDataItem> partialResults, EmailFilter filter)
        {
            if (filter != null && filter.OrganizationIdentifier != null)
            {
                foreach (SearchDataItem item in partialResults)
                {
                    if (string.IsNullOrEmpty(item.EmailTo))
                        continue;

                    var to = JsonConvert.DeserializeObject<Dictionary<Guid,string>>(item.EmailTo);

                    if (to != null && to.Count == 1)
                    {
                        item.UserTo = to.Single().Value;
                        item.UserToIdentifier = to.Single().Key;
                    }
                }
            }
            return partialResults;
        }

        #endregion

        protected static string DateToHtml(object date)
            => (date == null) ? string.Empty : TimeZones.Format((DateTimeOffset)date, User.TimeZone, true);

        protected static string GetAllMailto(object emailArra)
        {
            if (emailArra == null)
                return string.Empty;
            StringBuilder results = new StringBuilder();
            foreach (var email in (emailArra.ToString().Split(';')))
            {
                results.Append($"<a href='mailto:{email}'>{email}</a>");
            }
            return results.ToString();
        }
    }
}