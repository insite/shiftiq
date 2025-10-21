using System;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Developers.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/accounts/developers/edit";
        private const string SearchUrl = "/ui/admin/accounts/developers/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanCreate)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(Page);

            DeveloperDetails.SetDefaultInputValues(QPersonSecret.CreateTokenSecret());

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var secret = new QPersonSecret
            {
                SecretIdentifier = UniqueIdentifier.Create(),
            };

            DeveloperDetails.GetInputValues(secret);

            ServiceLocator.SendCommand(new AddPersonSecret(
                secret.PersonIdentifier,
                secret.SecretIdentifier,
                secret.SecretType,
                secret.SecretName,
                secret.SecretValue,
                secret.SecretExpiry,
                secret.SecretLifetimeLimit
            ));

            HttpResponseHelper.Redirect($"{EditUrl}?token={secret.SecretIdentifier}&status=saved");
        }
    }
}