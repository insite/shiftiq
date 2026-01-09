using System;

using InSite.Web.Security;

namespace InSite.UI.Portal.Accounts.Users
{
    public partial class ViewLicense : Layout.Lobby.LobbyBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BindModelToControls();
        }

        private void BindModelToControls()
        {
            ((Layout.Lobby.LobbyForm)Master).SetMainWidth(12);

            LicenseContainer.InnerHtml = AccountHelper.GetLicenseAgreement(false, false);
        }
    }
}