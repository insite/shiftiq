using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Web.Data;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Lobby
{
    public partial class Subscribe : Layout.Lobby.LobbyBasePage
    {
        private SubscribeModel Model
        {
            get => (SubscribeModel)ViewState[nameof(SubscribeModel)];
            set => ViewState[nameof(SubscribeModel)] = value;
        }

        private Guid? UserIdentifier => Guid.TryParse(Request.QueryString["user"], out var userId) ? userId : (Guid?)null;

        private bool IsResubscribe => Request.QueryString["resubscribe"] == "1";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubscribeButton.Click += SubscribeButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (IsResubscribe && UserIdentifier.HasValue)
                Resubscribe();

            Title = LabelHelper.GetTranslation(ActionModel.ActionName);

            CreateModel();

            BindModelToControls();
        }

        private void CreateModel()
        {
            Model = new SubscribeModel();

            var filter = new QGroupFilter
            {
                OrganizationIdentifier = Organization.Key,
                GroupType = GroupTypes.List,
                AllowSelfSubscription = true
            };

            var data = ServiceLocator.GroupSearch.GetGroups(filter);

            Model.Groups = data.Select(x => new SubscribeModel.GroupListItem
            {
                Identifier = x.GroupIdentifier,
                Name = x.GroupName
            }).ToArray();

            if (UserIdentifier.HasValue)
            {
                var groupKeys = Model.Groups.Select(x => x.Identifier).ToArray();
                var roles = MembershipSearch
                    .Bind(x => x.GroupIdentifier, x => x.UserIdentifier == UserIdentifier.Value && groupKeys.Contains(x.GroupIdentifier))
                    .ToHashSet();

                foreach (var group in Model.Groups)
                    group.Selected = roles.Contains(group.Identifier);
            }
        }

        private void BindModelToControls()
        {
            InputPanel.Visible = false;

            var user = UserIdentifier.HasValue
                ? UserSearch.Select(UserIdentifier.Value)
                : null;

            if (user == null)
                return;

            var separator = user.Email.IndexOf("@");
            if (separator >= 1)
            {
                var name = user.Email.Substring(0, separator);
                var domain = user.Email.Substring(separator + 1);

                UserEmail.Text = $"{name.First()}*****{name.Last()}@{domain}";
            }
            UserName.Text = user.FirstName + " *****";

            InputPanel.Visible = true;

            GroupList.DataSource = Model.Groups;
            GroupList.DataBind();

            BindMarketingEmailDisabled();
        }

        private void BindMarketingEmailDisabled()
        {
            var person = ServiceLocator.PersonSearch.GetPerson(UserIdentifier.Value, Organization.Identifier);
            if (person == null)
            {
                MarketingEmailDisabled.Visible = false;
                return;
            }

            MarketingEmailDisabled.Checked = !person.MarketingEmailEnabled;
            MarketingEmailDisabled.Text = $"Unsubscribe from all marketing emails from {Organization.CompanyName}";
        }

        private void SubscribeButton_Click(object sender, EventArgs e)
        {
            if (UserIdentifier == null)
                HttpResponseHelper.Redirect("/ui/lobby/signin");

            Guid user = UserIdentifier.Value;

            var person = ServiceLocator.PersonSearch.GetPerson(user, Organization.Identifier, x => x.User);
            if (person == null)
                return;

            var marketingEmailEnabled = !MarketingEmailDisabled.Checked;

            if (!person.EmailEnabled && marketingEmailEnabled)
                ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.EmailEnabled, true));

            if (person.MarketingEmailEnabled != marketingEmailEnabled)
                ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.MarketingEmailEnabled, marketingEmailEnabled));

            var subscribed = new List<string>();
            var unsubscribed = new List<string>();

            foreach (RepeaterItem item in GroupList.Items)
            {
                var groupSelector = (ICheckBox)item.FindControl("GroupIdentifier");
                var groupIdentifier = Guid.Parse(groupSelector.Value);

                if (groupSelector.Checked && !marketingEmailEnabled)
                    groupSelector.Checked = false;

                if (groupSelector.Checked)
                {
                    if (MembershipPermissionHelper.CanModifyMembership(groupIdentifier))
                    {
                        MembershipHelper.Save(groupIdentifier, user, null);
                        subscribed.Add(groupSelector.Text);
                    }
                }
                else
                {
                    MembershipStore.Delete(MembershipSearch.Select(groupIdentifier, user));
                    unsubscribed.Add(groupSelector.Text);
                }
            }

            var subject = "Your subscriptions have been updated.";

            Status.AddMessage(AlertType.Success, Translate(subject));

            if (!marketingEmailEnabled && person.MarketingEmailEnabled)
                Unsubscribed(person.User.Email);
            else
                SubscriptionUpdated(person.User.FullName, subscribed, unsubscribed);
        }

        private void Unsubscribed(string userEmail)
        {
            var domain = ServiceLocator.AppSettings.Security.Domain;
            var relativeUrl = $"/ui/lobby/subscribe?user={UserIdentifier}&resubscribe=1";
            var absoluteUrl = UrlHelper.GetAbsoluteUrl(domain, ServiceLocator.AppSettings.Environment, relativeUrl, Organization.Code);

            var alert = new AlertUnsubscribeSuccess
            {
                UserEmail = userEmail,
                Organization = Organization.CompanyName,
                ResubscribeUrl = absoluteUrl
            };

            var senderUserId = CurrentSessionState.Identity?.User?.Identifier ?? UserIdentifiers.Someone;

            try
            {
                ServiceLocator.AlertMailer.Send(
                    Organization.OrganizationIdentifier,
                    UserIdentifier ?? throw new ArgumentNullException(nameof(UserIdentifier)),
                    alert
                );
            }
            catch (MessageNotFoundException ex)
            {
                AppSentry.SentryError(ex);
            }
        }

        private void SubscriptionUpdated(string fullName, List<string> subscribed, List<string> unsubscribed)
        {
            ServiceLocator.AlertMailer.Send(
                new UserEmailSubscriptionModifiedNotification
                {
                    OriginOrganization = Organization.Identifier,
                    OriginUser = UserIdentifier,
                    RecipientName = fullName,
                    RecipientMemberships = CreateBody(subscribed, unsubscribed)
                },
                UserIdentifier
            );
        }

        private void Resubscribe()
        {
            Guid user = UserIdentifier.Value;

            var person = ServiceLocator.PersonSearch.GetPerson(user, Organization.Identifier, x => x.User);
            if (person != null)
            {
                if (!person.EmailEnabled)
                    ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.EmailEnabled, true));

                if (!person.MarketingEmailEnabled)
                    ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.MarketingEmailEnabled, true));
            }

            HttpResponseHelper.Redirect($"/ui/lobby/subscribe?user={user}");
        }

        private string CreateBody(List<string> subscribed, List<string> unsubscribed)
        {
            var body = new StringBuilder();

            if (subscribed.Count > 0)
            {
                body.AppendLine("You are subscribed to these mailing lists:");
                foreach (var group in subscribed)
                    body.AppendLine($"- {group}");
                body.AppendLine();
            }

            if (unsubscribed.Count > 0)
            {
                body.AppendLine("You are not subscribed to these mailing lists:");
                foreach (var group in unsubscribed)
                    body.AppendLine($"- {group}");
                body.AppendLine();
            }

            return body.ToString();
        }
    }
}