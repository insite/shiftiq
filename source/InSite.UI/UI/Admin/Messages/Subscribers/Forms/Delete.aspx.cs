using System;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.UI.Admin.Messages.Subscribers.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private const string SearchUrl = "/ui/admin/messages/messages/search";

        private string OutlineUrl => $"/ui/admin/messages/outline?message={MessageID}";

        private Guid MessageID => Guid.TryParse(Request["message"], out var result) ? result : Guid.Empty;

        private Guid RecipientID => Guid.TryParse(Request["recipient"], out var result) ? result : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
            CancelButton.NavigateUrl = $"/ui/admin/messages/outline?message={MessageID}";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var recipient = ServiceLocator.MessageSearch.GetSubscriberUser(MessageID, RecipientID);
            if (recipient == null)
                HttpResponseHelper.Redirect(OutlineUrl);

            var message = ServiceLocator.MessageSearch.GetMessage(MessageID);
            if (message == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, null, "From " + message.MessageName);

            Subscribed.Text = GetLocalTime(recipient.Subscribed);

            var people = ServiceLocator.PersonSearch.GetPersons(new QPersonFilter
            {
                UserIdentifier = RecipientID,
                OrganizationOrParentOrganizationIdentifier = Organization.Key
            }, x => x.User);

            var person = people.FirstOrDefault(x => x.OrganizationIdentifier == Organization.Key)
                ?? people.FirstOrDefault(x => x.OrganizationIdentifier != Organization.Key);

            if (person != null)
            {
                PersonDetail.BindPerson(person, User.TimeZone);
                SubscriberCount.Text = "1";
            }
            else
            {
                DeleteButton.Visible = false;
                SubscriberCount.Text = "0";
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new RemoveMessageSubscriber(MessageID, RecipientID, false));

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        private string GetLocalTime(DateTimeOffset? item)
        {
            return item.Format(User.TimeZone, nullValue: "None", isHtml: true);
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"message={MessageID}"
                : null;
        }
    }
}