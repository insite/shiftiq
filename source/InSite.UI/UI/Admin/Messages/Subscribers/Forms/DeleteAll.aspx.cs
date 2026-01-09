using System;
using System.Linq;

using InSite.Application.Messages.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Messages.Subscribers.Forms
{
    public partial class DeleteAll : AdminBasePage, IHasParentLinkParameters
    {
        private Guid MessageID => Guid.TryParse(Request["message"], out var result) ? result : Guid.Empty;

        private Guid? SurveyID
        {
            get => (Guid?)ViewState[nameof(SurveyID)];
            set => ViewState[nameof(SurveyID)] = value;
        }

        private string MessageType
        {
            get => ViewState[nameof(MessageType)] as string;
            set => ViewState[nameof(MessageType)] = value;
        }

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

            var message = ServiceLocator.MessageSearch.GetMessage(MessageID);

            PageHelper.AutoBindHeader(this, null, "From " + message.MessageName);

            MessageSubject.Text = $"<a href=\"/ui/admin/messages/outline?message={MessageID}\">{message.MessageName}</a>";

            var subscribers = ServiceLocator.MessageSearch.GetSubscriberUsers(MessageID);

            SubscribersCount.Text = subscribers.Count.ToString();

            Grid.DataSource = subscribers.ToSearchResult();
            Grid.DataBind();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var subscribers = ServiceLocator.MessageSearch
                .GetSubscriberUsers(MessageID)
                .Select(x => x.UserIdentifier)
                .Distinct()
                .ToList();

            ServiceLocator.SendCommand(new RemoveMessageSubscribers(MessageID, subscribers, false));

            HttpResponseHelper.Redirect($"/ui/admin/messages/outline?message={MessageID}");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"message={MessageID}"
                : null;
        }
    }
}