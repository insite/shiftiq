using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Users
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid? UserIdentifier => Guid.TryParse(Request["user"], out var result) ? result : (Guid?)null;

        private User Entity
        {
            get
            {
                if (!_isEntityLoaded)
                {
                    _entity = UserIdentifier.HasValue ? UserSearch.Select(UserIdentifier.Value) : null;
                    _isEntityLoaded = true;
                }

                return _entity;
            }
        }

        #endregion

        #region Fields

        private User _entity;
        private bool _isEntityLoaded;

        #endregion

        #region Initialization and Loading

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
                if (Entity == null)
                    RedirectToFinder();

                PageHelper.AutoBindHeader(
                    Page,
                    qualifier: $"{Entity.FullName} <span class='form-text'>{Entity.Email}</span>");

                BindDetails(Entity);

                CancelButton.NavigateUrl = CloseButton.NavigateUrl = $"/ui/admin/accounts/users/edit?contact={Entity.UserIdentifier}";

                CheckIsValid();
            }
        }
        #endregion

        #region Event handlers

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!CheckIsValid())
                return;

            try
            {
                if (Entity != null)
                {
                    UserStore.Delete(Entity.UserIdentifier);

                    if (!string.IsNullOrEmpty(Entity.ImageUrl))
                        FileHelper.Provider.Delete(CurrentSessionState.Identity.Organization.Identifier, Entity.ImageUrl);
                }
            }
            catch (SqlException sqlex)
            {
                DisplayError(sqlex);
                return;
            }

            RedirectToFinder();
        }

        #endregion

        #region Methods (Data Sources)


        public void BindDetails(User user)
        {
            UserName.Text = $"<a href=\"/ui/admin/accounts/users/edit?contact={user.UserIdentifier}\">{user.FullName}</a>";
            Email.Text = $"<a href='mailto:{user.Email}'>{user.Email}</a>";
        }

        #endregion

        #region Helper methods

        private bool CheckIsValid()
        {
            if (Entity == null)
                return ShowError("User not found.");

            var organizationsCount = ServiceLocator.PersonSearch.CountPersons(new QPersonFilter { UserIdentifier = Entity.UserIdentifier });
            OrganizationsCount.Text = organizationsCount.ToString();

            var isSignedIn = CookieTokenModule.GetActiveTokens()
                .Any(x => string.Equals(x.UserEmail, Entity.Email, StringComparison.OrdinalIgnoreCase)
                      || string.Equals(x.ImpersonatorUser, Entity.Email, StringComparison.OrdinalIgnoreCase));

            if (Entity.UserIdentifier == CurrentSessionState.Identity.User.UserIdentifier || isSignedIn)
                return ShowError("Can't delete the user until the user has signed out.");

            if (organizationsCount > 0)
                return ShowError("Can't delete the user until contact person account(s) is (are) not deleted from organization(s).");

            return true;

            bool ShowError(string html)
            {
                ConfirmMessage.InnerHtml = "<i class=\"fas fa-stop-circle\"></i> " + html;

                DeleteButton.Visible = CancelButton.Visible = false;
                CloseButton.Visible = true;

                return false;
            }
        }

        private void RedirectToFinder() =>
            HttpResponseHelper.Redirect("/ui/admin/accounts/users/search");

        private void DisplayError(SqlException sqlex)
        {
            ConfirmMessage.Visible = DeleteButton.Visible = CancelButton.Visible = false;
            ErrorPanel.Visible = CloseButton.Visible = true;

            ErrorText.Text = sqlex.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint")
                ? ParseExceptionMessage(sqlex)
                : "An error occurred on the server side.";

            AppSentry.SentryError(sqlex);
        }

        private string ParseExceptionMessage(SqlException sqlex)
        {
            Regex pattern = new Regex(
                "The DELETE statement conflicted with the REFERENCE constraint \"(?<FkName>.+?)\". "
              + "The conflict occurred in database \"(?<DbName>.+?)\", table \"(?<TableName>.+?)\", column '(?<ColumnName>.+?)'.",
            RegexOptions.Compiled);

            try
            {
                var error = new StringBuilder("Database contains the records that reference this contact");

                var matches = pattern.Matches(sqlex.Message);
                if (matches.Count > 0)
                {
                    error.Append(": <ul>");

                    foreach (Match match in matches)
                        error.Append("<li>").Append(match.Groups["TableName"].Value).Append(" (")
                            .Append(match.Groups["ColumnName"].Value).Append(")</li>");

                    error.Append("</ul>");
                }
                else
                {
                    error.Append(".");
                }

                error.Append("You must manually remove them before you can delete the contact.");

                return error.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/edit") ? $"contact={UserIdentifier}" : null;

        #endregion
    }
}