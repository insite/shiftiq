using System;
using System.Web;

using InSite.Application.Credentials.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Achievements.Credentials.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? CredentialID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        private string ReturnUrl => Request["return"].HasValue() ? HttpUtility.UrlDecode(Request["return"]) : null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var credential = CredentialID.HasValue ? ServiceLocator.AchievementSearch.GetCredential(CredentialID.Value) : null;

                if (credential == null || credential.OrganizationIdentifier != Organization.Identifier)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/credentials/search");
                    return;
                }
                var title = $"{credential.AchievementTitle} <span class=\"form-text\" style=\"padding-right:10px;\">for</span> {credential.UserFullName}";
                var outlineUrl = $"/ui/admin/records/credentials/outline?id={CredentialID.Value}";

                PageHelper.AutoBindHeader(this, null, title);

                CredentialDetails.BindCredential(credential, User.TimeZone);

                CancelButton.NavigateUrl = outlineUrl;

                if (ReturnUrl.HasValue())
                {
                    if (ReturnUrl.Contains("ui/admin/records/achievements/outline") ||
                        ReturnUrl.Contains("ui/admin/contacts/people/edit") ||
                        ReturnUrl.Contains("ui/admin/records/gradebooks/outline"))
                    {
                        if (!ReturnUrl.Contains("panel=credentials"))
                            CancelButton.NavigateUrl = $"{ReturnUrl}&panel=credentials";
                        else
                            CancelButton.NavigateUrl = ReturnUrl;
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var credential = ServiceLocator.AchievementSearch.GetCredential(CredentialID.Value);

            if (credential != null)
            {
                ServiceLocator.SendCommand(new DeleteCredential(credential.CredentialIdentifier));
            }

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
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