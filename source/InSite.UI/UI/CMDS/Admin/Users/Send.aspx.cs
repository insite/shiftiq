using System;
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
                PersonHelper.SendWelcomeMessage(Organization.Identifier, PersonID.Value, CarbonCopy(Cc.Text));

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

        private Guid[] CarbonCopy(string text)
        {
            var thumbprint = UserSearch.BindFirst(x => x.UserIdentifier, new UserFilter
            {
                EmailExact = text,
                IsCmds = true
            });
            if (thumbprint != Guid.Empty)
                return new Guid[] { thumbprint };
            return null;

        }

        private bool AreEmailAddressesValid()
        {
            var message = new StringBuilder();

            AddEmailIfInvalid(To.Text, message);
            AddEmailIfInvalid(Cc.Text, message);

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