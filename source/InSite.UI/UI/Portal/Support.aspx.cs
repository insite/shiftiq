using System;
using System.Linq;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Individual
{
    public partial class Support : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SendButton.Click += SendButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            EnforceAuthentication();
            ShowAdministratorInstruction();

            if (!IsPostBack)
                BindModelToControls();
        }

        private void EnforceAuthentication()
        {
            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();
        }

        private void ShowAdministratorInstruction()
        {
            if (ServiceLocator.Partition.IsE03())
                return;

            var jira = ServiceLocator.AppSettings.Integration.Jira;
            if (!jira.Enabled)
                return;

            var person = PersonSearch.Select(Organization.OrganizationIdentifier, User.UserIdentifier, x => x.User);
            if (!person.IsAdministrator)
                return;

            var message = $"{person.User.FirstName}, you are signed in with an <strong>administrator</strong> account. If you'd like to"
                + $" request assistance from the Shift iQ team, then please visit the"
                + $" <a href='{jira.ServiceDeskUrl}'>Shift iQ service desk</a>."
                + $" Otherwise, you can send a help request to your own support team by submitting the form below.";

            Alert.AddMessage(AlertType.Information, message);
        }

        private void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);

            var person = PersonSearch.Select(Organization.OrganizationIdentifier, User.UserIdentifier, x => x.EmployerGroup);

            FromFullName.Text = User.FullName;
            FromEmail.Text = User.Email;
            Phone.Text = User.Phone;
            EmployerProgram.Text = person.EmployerGroup?.GroupName ?? Organization.LegalName;

            CloseButton.NavigateUrl = RelativeUrl.PortalHomeUrl;

            var supportPage = ServiceLocator.PageSearch.Bind(x => x, x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.PageSlug == "support" && x.Parent == null)
                .ToList()
                .FirstOrDefault();

            if (supportPage != null)
            {
                var content = ServiceLocator.ContentSearch.GetBlock(supportPage.PageIdentifier, CurrentLanguage);
                if (content != null)
                {
                    var title = content.Title?.GetText();
                    var summary = content.Summary?.GetText();
                    var body = content.Body.GetHtml();

                    if (body != null)
                    {
                        PortalSupportHtml.Text = body;
                        PortalSupportContent.Visible = true;
                    }
                }
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.AlertMailer.Send(Organization.Identifier, User.UserIdentifier, CreateAlert());
            Alert.AddMessage(AlertType.Success, GetDisplayText("Portal Support Submitted", null));
            SendButton.Enabled = false;
        }

        private AlertHelpRequested CreateAlert()
        {
            var address = Request.UserHostAddress;
            var browser = Request.Browser;
            var help = new AlertHelpRequested
            {
                BrowserAddress = address,
                BrowserName = $"{browser.Platform} {browser.Browser} {browser.Version}",
                Organization = Organization.LegalName,
                RequestDescription = Message.Text,
                RequestSource = "InSite Portal",
                RequestSummary = Summary.Text,
                RequestUrl = $"{Request.Url.Scheme}://{Request.Url.Host}{Request.QueryString["ref"] ?? string.Empty}",
                UserEmail = User.Email,
                UserName = User.FullName,
                BrowserUrl = $"{Request.Url.Scheme}://{Request.Url.Host}{Request.QueryString["ref"] ?? string.Empty}",
                UserPhone = GetUserPhone()
            };

            string GetUserPhone()
            {
                if (Phone.Text.HasValue())
                    return Phone.Text;

                return User.Phone != null ? User.Phone.ToString() : "n/a";
            }

            var person = PersonSearch.Select(Organization.OrganizationIdentifier, User.UserIdentifier, x => x.EmployerGroup);
            help.UserCompany = person.EmployerGroup?.GroupName ?? Organization.LegalName;
            help.UserEmployer = EmployerProgram.Text ?? "Not Specified";

            return help;
        }
    }
}