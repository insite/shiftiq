using System;
using System.Linq;
using System.Text;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Admin.People.Forms
{
    public partial class Send : AdminBasePage, ICmdsUserControl
    {
        private Guid? PersonID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (!AreEmailAddressesValid())
                return;
            try
            {
                PersonHelper.SendWelcomeMessage(Organization.Identifier, PersonID.Value,
                    CarbonCopy(Cc.Text), subject: Subject.Text, body: MessageBody.Text);

                HttpResponseHelper.Redirect($"/ui/cmds/admin/users/edit?userID={PersonID}", true);
            }
            catch (Exception ex)
            {
                SendStatus.AddMessage(AlertType.Error, ex.Message);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SendButton.Click += SendButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BindOutboxWarning();

            if (IsPostBack)
                return;

            if (!PersonID.HasValue)
                HttpResponseHelper.Redirect("/ui/cmds/admin/users/edit", true);

            PageHelper.AutoBindHeader(this);

            try
            {
                PageHelper.AutoBindHeader(this, null, "Send Welcome Email");

                var email = PersonID.HasValue ? UserSearch.Bind(PersonID.Value, x => x.Email) : null;
                var welcomed = PersonHelper.CreateWelcomeMessage(Organization.Identifier, PersonID.Value, false);

                var message = ServiceLocator.CoreProcessManager.CreateEmail(welcomed, OrganizationIdentifiers.CMDS);
                if (message.Recipients.Count > 0)
                    To.Text = email;

                Subject.Text = message.ContentSubject.Default;
                MessageBody.Text = message.ContentBody.Default;

                CancelButton.NavigateUrl = $"/ui/cmds/admin/users/edit";
            }
            catch (InvalidEmailAddressException ex)
            {
                SendStatus.AddMessage(AlertType.Error, ex.Message);
                SendButton.Visible = false;
            }
        }

        private void BindOutboxWarning()
        {
            var application = ServiceLocator.AppSettings.Application;

            var isEnabled = string.Compare(application.EmailOutbox, "Enabled", true) == 0;
            if (isEnabled)
                return;

            string message;

            if (application.EmailOutboxDisabled)
            {
                message = "The email outbox is disabled in this environment. The Send button records the message but no email is delivered to anyone.";
            }
            else if (application.EmailOutboxFiltered)
            {
                var whitelist = string.Join(", ", new[]
                {
                    ServiceLocator.Partition.WhitelistDomains,
                    ServiceLocator.Partition.WhitelistEmails
                }.Where(x => x.HasValue()));

                message = whitelist.HasValue()
                    ? $"The email outbox is filtered in this environment. The Send button delivers the message only to whitelisted recipients ({whitelist}). Every other address is dropped."
                    : "The email outbox is filtered in this environment. The Send button delivers the message only to whitelisted recipients. Every other address is dropped.";
            }
            else
            {
                message = $"The email outbox is set to \"{application.EmailOutbox}\", not Enabled. The Send button may not deliver the message.";
            }

            OutboxWarning.AddMessage(AlertType.Warning, message);
        }

        private Guid[] CarbonCopy(string text)
        {
            var user = FindCarbonCopyUser(text);

            return user.HasValue ? new[] { user.Value } : null;
        }

        // The outbox copies a user only through a person in this organization (any organization
        // when this one is the partition) whose email is enabled, and drops anyone else without a
        // word. Validation and the send share this lookup, so an address that passes is one that
        // is copied.
        private Guid? FindCarbonCopyUser(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var organization = Organization.Identifier;

            var user = UserSearch.BindFirst(x => x.UserIdentifier, new UserFilter
            {
                EmailExact = email,
                IsCmds = true,
                PersonOrganizationIdentifier = organization == ServiceLocator.Partition.Identifier
                    ? (Guid?)null
                    : organization,
                PersonEmailEnabled = true
            });

            return user != Guid.Empty ? user : (Guid?)null;
        }

        private bool AreEmailAddressesValid()
        {
            var message = new StringBuilder();

            AddEmailIfInvalid(To.Text, message);
            AddEmailIfInvalid(Cc.Text, message);

            if (message.Length == 0 && Cc.Text.IsNotEmpty() && FindCarbonCopyUser(Cc.Text) == null)
                message.AppendFormat(
                    "Cannot copy {0}: the address must belong to a CMDS user in this organization "
                    + "whose email is enabled.", Cc.Text);

            if (message.Length == 0)
                return true;

            SendStatus.AddMessage(AlertType.Error, message.ToString());

            return false;
        }

        private static void AddEmailIfInvalid(string emailAddress, StringBuilder message)
        {
            if (string.IsNullOrEmpty(emailAddress))
                return;

            if (!EmailAddress.IsValidAddress(emailAddress))
            {
                if (message.Length > 0)
                    message.Append("<br/>");

                message.AppendFormat("Invalid email address: {0}", emailAddress);
            }
            else if (!UserSearch.Exists(new UserFilter { EmailExact = emailAddress }))
            {
                if (message.Length > 0)
                    message.Append("<br/>");

                message.AppendFormat("There is no CMDS user registered with the email address: {0}", emailAddress);
            }
        }
    }
}