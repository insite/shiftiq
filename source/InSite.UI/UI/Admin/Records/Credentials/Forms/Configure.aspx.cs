using System;
using System.Web.UI;

using InSite.Application.Credentials.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Achievements.Credentials.Forms
{
    public partial class Configure : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CredentialID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;

            if (ServiceLocator.Partition.IsE03())
            {
                Necessity.Settings.OrganizationIdentifier = OrganizationIdentifiers.CMDS;
                Priority.Settings.OrganizationIdentifier = OrganizationIdentifiers.CMDS;
            }
            else
            {
                Necessity.Settings.UseCurrentOrganization = true;
                Priority.Settings.UseCurrentOrganization = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var credential = CredentialID.HasValue ? ServiceLocator.AchievementSearch.GetCredential(CredentialID.Value) : null;
            if (credential == null || credential.OrganizationIdentifier != Organization.OrganizationIdentifier)
                HttpResponseHelper.Redirect($"/ui/admin/records/credentials/search");

            var title = $"{credential.AchievementTitle} <span class=\"form-text\">for</span> {credential.UserFullName}";

            PageHelper.AutoBindHeader(this, null, title);

            CredentialDetails.BindCredential(credential, User.TimeZone);
            ExpirationField.SetExpiration(credential);

            Necessity.Value = credential.CredentialNecessity;
            Priority.Value = credential.CredentialPriority;

            CancelButton.NavigateUrl = $"/ui/admin/records/credentials/outline?id={CredentialID}";
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var expiration = ExpirationField.GetExpiration();
            var credential = ServiceLocator.AchievementSearch.GetCredential(CredentialID.Value);

            ServiceLocator.SendCommand(new ChangeCredentialExpiration(credential.CredentialIdentifier, expiration));
            ServiceLocator.SendCommand(new TagCredential(credential.CredentialIdentifier, Necessity.Value, Priority.Value));

            HttpResponseHelper.Redirect($"/ui/admin/records/credentials/outline?id={CredentialID}");
        }

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={CredentialID}"
                : null;
        }

        #endregion
    }
}
