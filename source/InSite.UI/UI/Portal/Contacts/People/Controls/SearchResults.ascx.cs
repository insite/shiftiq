using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Lobby;

using Shift.Common;
using Shift.Common.Events;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<PersonFilter>
    {
        #region Events

        public event AlertHandler Alert;

        private void OnAlert(AlertType indicator, string message) =>
            Alert?.Invoke(this, new AlertArgs(indicator, message));

        #endregion

        #region Classes

        public class ExportDataItem
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public int SessionCount { get; set; }
            public DateTimeOffset? LastAuthenticated { get; set; }
        }

        #endregion

        #region Properties

        // The user-administration row actions (password reset, welcome email, impersonate) belong
        // to the design/users permission. A Portal/Contacts-only caller never sees the Action column.
        private bool AllowUserActions
        {
            get => (bool?)ViewState[nameof(AllowUserActions)] ?? false;
            set => ViewState[nameof(AllowUserActions)] = value;
        }

        // Impersonation is restricted by its own action permission, checked against the caller's groups.
        private bool AllowImpersonation
        {
            get => (bool?)ViewState[nameof(AllowImpersonation)] ?? false;
            set => ViewState[nameof(AllowImpersonation)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCommand += Grid_RowCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var personCodeColumn = Grid.Columns.FindByName("PersonCode");

            personCodeColumn.HeaderText = LabelHelper.GetLabelContentText("Person Code");

            AllowUserActions = Identity.IsGranted(PersonSearchGate.UserAdministrationResource);

            AllowImpersonation = AllowUserActions && TGroupPermissionSearch.AllowImpersonation(Identity.Groups);

            foreach (var column in Grid.Columns.FindAllByHeaderText("Action"))
                column.Visible = AllowUserActions;

            // The session count belongs to the design/users permission, like its filter.
            foreach (var column in Grid.Columns.FindAllByHeaderText("Sessions"))
                column.Visible = AllowUserActions;
        }

        #endregion

        #region Event handlers

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SendPasswordReset" && e.CommandName != "SendWelcomeEmail")
                return;

            // Hiding the Action column is not enough: a crafted postback can still raise the
            // command, so refuse it here for a caller without the user-administration permission.
            if (!AllowUserActions)
                return;

            var organizationId = Organization.Identifier;
            var userId = Grid.GetDataKey<Guid>(e);
            var user = PersonCriteria.BindFirst(x => x.User, new PersonFilter
            {
                OrganizationIdentifier = organizationId,
                UserIdentifier = userId
            });

            if (e.CommandName == "SendPasswordReset")
            {
                var tokenId = ResetTokenFile.Create(user);

                var url = $"{Common.Web.HttpRequestHelper.CurrentRootUrl}{Lobby.ResetPassword.GetUrl()}?token={tokenId}";

                ServiceLocator.AlertMailer.Send(Organization.Identifier, user.UserIdentifier, new Domain.Messages.AlertPasswordResetRequested
                {
                    ResetUrl = url
                });

                OnAlert(AlertType.Success, $"{Translate("Password reset has been sent for")} {user.FullName}");
            }
            else if (e.CommandName == "SendWelcomeEmail")
            {
                PersonHelper.SendWelcomeMessage(organizationId, user.UserIdentifier);

                OnAlert(AlertType.Success, $"{Translate("Welcome email has been sent for")} {user.FullName}");
            }
        }

        #endregion

        #region Methods (data binding)

        protected override int SelectCount(PersonFilter filter)
        {
            return PersonCriteria.Count(filter);
        }

        protected override IListSource SelectData(PersonFilter filter)
        {
            return PersonCriteria.SelectSearchResults(filter).ToSearchResult();
        }

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(PersonFilter filter, bool empty)
        {
            return PersonCriteria.SelectSearchResults(filter)
                .Select(x => new ExportDataItem
                {
                    Name = x.FullName,
                    Email = x.Email,
                    SessionCount = x.SessionCount,
                    LastAuthenticated = x.LastAuthenticated
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Methods (render)

        protected string GetInitials()
        {
            var item = (PersonSearchResultItem)Page.GetDataItem();
            return $"{item.FirstName[0]}{item.LastName[0]}".ToUpper();
        }

        protected string GetImpersonateIcon(Guid user, bool isApproved)
        {
            if (!AllowImpersonation)
                return string.Empty;

            var url = Urls.StartImpersonation + $"?user={user}";

            return isApproved
                ? $"<a class='btn btn-sm btn-default' title='{Translate("Impersonate")}' href='{url}'><i class='fas fa-user-secret'></i></a>"
                : $"<span class='btn btn-sm btn-default' title='{Translate("Not Approved")}' class='text-danger'><i class='fas fa-user-secret'></i></span>";
        }

        #endregion
    }
}
