using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Admin.Contacts.People.Utilities;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Lobby;

using Shift.Common.Linq;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.UI.Desktops.Design.Users.Controls
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

        private class SearchDataItem
        {
            public Guid UserIdentifier { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string EmailAlternate { get; set; }
            public int SessionCount { get; set; }
            public bool IsApproved { get; set; }
            public DateTimeOffset? LastAuthenticated { get; set; }
        }

        #endregion

        #region Properties

        private bool AllowImpersonation
        {
            get => (bool)ViewState[nameof(AllowImpersonation)];
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

            if (!IsPostBack)
                AllowImpersonation = TGroupPermissionSearch.AllowImpersonation(Identity.Groups);
        }

        #endregion

        #region Event handlers

        private void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
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
            => PersonCriteria.Count(filter);

        protected override IListSource SelectData(PersonFilter filter)
        {
            var items = PersonCriteria.SelectSearchResults(filter)
                .Select(x => new SearchDataItem
                {
                    UserIdentifier = x.UserIdentifier,
                    FullName = x.FullName,
                    Email = x.Email,
                    EmailAlternate = x.EmailAlternate,
                    SessionCount = x.SessionCount,
                    IsApproved = x.IsApproved,
                    LastAuthenticated = x.LastAuthenticated
                }).ToList();

            return items.ToSearchResult();
        }

        #endregion

        #region Methods (export)

        public override IListSource GetExportData(PersonFilter filter, bool empty)
        {
            return SelectData(filter)
                .GetList()
                .Cast<SearchDataItem>()
                .Select(x => new ExportDataItem
                {
                    Name = x.FullName,
                    Email = x.Email,
                    SessionCount = x.SessionCount,
                    LastAuthenticated = x.LastAuthenticated,
                })
                .ToList()
                .ToSearchResult();
        }

        #endregion

        #region Methods (render)

        protected string GetImpersonateIcon(Guid user, bool isApproved)
        {
            if (!AllowImpersonation)
                return string.Empty;

            return isApproved
                ? $"<a class='btn btn-sm btn-default' title='{Translate("Impersonate")}' href='/ui/portal/identity/impersonate?user={user}'><i class='fas fa-user-secret'></i></a>"
                : $"<span class='btn btn-sm btn-default' title='{Translate("Not Approved")}' class='text-danger'><i class='fas fa-user-secret'></i></span>";
        }

        #endregion
    }
}