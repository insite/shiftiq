using System;

using InSite.Application.Contacts.Read;
using InSite.Application.People.Write;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Developers.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/accounts/developers/search";
        private const string EditUrl = "/ui/admin/accounts/developers/edit";

        private Guid TokenID => Guid.TryParse(Request.QueryString["token"], out var value) ? value : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        public override void ApplyAccessControl()
        {
            base.ApplyAccessControl();

            SaveButton.Visible = CanEdit;
            DeleteButton.Visible = CanDelete;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Request.QueryString["status"] == "saved")
                SetStatus(ScreenStatus, StatusType.Saved);

            Open();

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void Open()
        {
            var secret = ServiceLocator.PersonSecretSearch.GetSecret(TokenID,
                        x => x.Person,
                        x => x.Person.User,
                        x => x.Person.Organization);

            if (secret == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var secretUser = secret.Person?.User;

            if (secretUser == null)
                HttpResponseHelper.Redirect(SearchUrl);

            var user = UserSearch.Select(secretUser.UserIdentifier);

            PageHelper.AutoBindHeader(Page, qualifier: user?.FullName ?? "Developer Token");

            DeveloperDetails.SetInputValues(secret);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var oldSecret = ServiceLocator.PersonSecretSearch.GetSecret(TokenID,
              x => x.Person,
              x => x.Person.User,
              x => x.Person.Organization);

            if (oldSecret?.Person == null)
                return;

            var personId = oldSecret.Person.PersonIdentifier;

            var newSecret = new QPersonSecret
            {
                SecretIdentifier = UniqueIdentifier.Create(),
                PersonIdentifier = personId
            };

            DeveloperDetails.GetInputValues(newSecret);

            ServiceLocator.SendCommand(new RemovePersonSecret(oldSecret.SecretIdentifier, TokenID));

            ServiceLocator.SendCommand(new AddPersonSecret(
                personId,
                newSecret.SecretIdentifier,
                newSecret.SecretType,
                newSecret.SecretName,
                newSecret.SecretValue,
                newSecret.SecretExpiry,
                newSecret.SecretLifetimeLimit
            ));

            HttpResponseHelper.Redirect($"{EditUrl}?token={newSecret.SecretIdentifier}&status=saved");
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var secret = ServiceLocator.PersonSecretSearch.GetSecret(TokenID,
              x => x.Person,
              x => x.Person.User,
              x => x.Person.Organization);

            if (secret?.Person == null)
                return;

            var personId = secret.Person.PersonIdentifier;

            ServiceLocator.SendCommand(new RemovePersonSecret(personId, TokenID));

            HttpResponseHelper.Redirect(SearchUrl);
        }
    }
}