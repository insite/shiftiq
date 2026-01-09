using System;
using System.Web.UI;

using InSite.Application.Credentials.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Achievements.Credentials.Forms
{
    public partial class Grant : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CredentialID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var credential = CredentialID.HasValue ? ServiceLocator.AchievementSearch.GetCredential(CredentialID.Value) : null;
                if (credential == null || credential.OrganizationIdentifier != Organization.OrganizationIdentifier)
                    HttpResponseHelper.Redirect($"/ui/admin/records/credentials/search");

                var title = $"{credential.AchievementTitle} <span class=\"form-text\">for</span> {credential.UserFullName}";

                PageHelper.AutoBindHeader(this, null, title);

                CredentialDetails.BindCredential(credential, User.TimeZone, false);

                GrantedDate.Value = credential.CredentialGranted;

                CancelButton.NavigateUrl = $"/ui/admin/records/credentials/outline?id={CredentialID}";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var credential = ServiceLocator.AchievementSearch.GetCredential(CredentialID.Value);
            var person = ServiceLocator.ContactSearch.GetPerson(credential.UserIdentifier, credential.OrganizationIdentifier);

            ServiceLocator.SendCommand(new GrantCredential(
                CredentialID.Value,
                GrantedDate.Value.Value,
                null,
                null,
                person?.EmployerGroupIdentifier,
                person?.EmployerGroupStatus));

            HttpResponseHelper.Redirect($"/ui/admin/records/credentials/outline?id={CredentialID}");
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={CredentialID}"
                : null;
        }
    }
}
