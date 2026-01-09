using System;
using System.Linq;
using System.Runtime.CompilerServices;

using InSite.Application.People.Write;
using InSite.Common;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.Web.Security;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby
{
    public partial class ResetPassword : Layout.Lobby.LobbyBasePage
    {
        #region Constants

        private const int MaxAttemptsNumber = 5;
        private const int MinAttemptsNumber = 0;

        #endregion

        #region Properties

        private int ResetAttemptsCount
        {
            get => (int?)GetSessionValue() ?? 0;
            set => SetSessionValue(value);
        }

        private bool IsResetBruteForceDetected => ResetAttemptsCount >= MaxAttemptsNumber;

        private string Token => Request.QueryString["token"];

        private Guid? TokenIdentifier
        {
            get => (Guid?)ViewState[nameof(TokenIdentifier)];
            set => ViewState[nameof(TokenIdentifier)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RequestButton.Click += RequestButton_Click;
            ChangeButton.Click += ChangeButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Title = LabelHelper.GetTranslation(ActionModel.ActionName);

            RequestButton.Text = LabelHelper.GetTranslation("Continue");
            ChangeButton.Text = LabelHelper.GetTranslation("Submit");

            if (string.IsNullOrEmpty(Token))
            {
                ScreenViews.SetActiveView(ViewRequest);

                RequestEmail.Text = Request.IsAuthenticated ? Identity.User.Email : string.Empty;

                return;
            }

            var exceptionSource = typeof(ResetPassword).FullName + "." + nameof(OnLoad);

            if (!Guid.TryParse(Token, out var tokenId))
            {
                ShowStatus(AlertType.Error, "far fa-times fs-xl", "ResetPassword.InvalidLink");
                return;
            }

            var tokenFile = ResetTokenFile.Read(tokenId);
            if (tokenFile != null && !tokenFile.IsValid(out var validationError))
            {
                AppSentry.SentryWarning(validationError, exceptionSource);

                tokenFile.Delete();
                tokenFile = null;
            }

            if (tokenFile == null)
            {
                var html = LabelHelper.GetTranslation("ResetPassword.ExpiredLink", true);

                ShowStatus(AlertType.Error, html, ResetTokenFile.ResetTokenFileExpirationTime);
                return;
            }

            ResetAttemptsCount = 0;

            ResetTokenFile.Clear(x => x.UserIdentifier == tokenFile.UserIdentifier && x.TokenID != tokenId);
            ScreenViews.SetActiveView(ViewChange);

            TokenIdentifier = tokenId;

            CancelButton.NavigateUrl = "/ui/lobby/signin";
        }

        #endregion

        #region Event handlers

        private void RequestButton_Click(object sender, EventArgs e)
        {
            var email = RequestEmail.Text;
            var exceptionSource = typeof(ResetPassword).FullName + "." + nameof(RequestButton_Click);

            if (string.IsNullOrEmpty(email))
            {
                AppSentry.SentryWarning("Invalid request: email is null.", exceptionSource);
                ShowStatus(AlertType.Error, Translate("Invalid request."));
                return;
            }

            if (IsResetBruteForceDetected)
            {
                ShowStatus(AlertType.Warning, "ResetPassword.BruteForceAttackMessage", MaxAttemptsNumber);
                return;
            }

            ResetAttemptsCount++;

            var user = UserSearch.SelectByEmail(email, x => x.Persons);

            if (user == null)
            {
                ShowStatus(AlertType.Error, "ResetPassword.InvalidEmail");
                return;
            }

            if (!user.Persons.Any(x => x.UserAccessGranted.HasValue))
            {
                var query = $"?ticket={Lobby.RequestAccess.CreateTicket(user.Email)}";
                var extra = $"<br/>You can <a href='/ui/lobby/request-access{query}'>request access here</a>.";
                ShowError("ResetPassword.NotApproved", extra);
                return;
            }

            var person = PersonSearch.Select(Organization.Identifier, user.UserIdentifier);
            if (person != null && !person.EmailEnabled)
                ServiceLocator.SendCommand(new ModifyPersonFieldBool(person.PersonIdentifier, PersonField.EmailEnabled, true));

            var tokenId = ResetTokenFile.GetOrCreateToken(user);

            try
            {
                var url = $"{HttpRequestHelper.CurrentRootUrl}{GetUrl()}?token={tokenId}";
                var userOrganizationIdentifier = user.Persons.Count > 0
                    ? user.Persons.Select(y => y.OrganizationIdentifier).First()
                    : OrganizationIdentifiers.Global;

                var alert = new AlertPasswordResetRequested()
                {
                    ResetUrl = url,
                    Type = NotificationType.PasswordResetRequested
                };

                var ids = ServiceLocator.AlertMailer.Send(Organization.Identifier, user.UserIdentifier, alert);

                var status = ids.IsNotEmpty()
                    ? TEmailSearch.Select(ids[0])
                    : null;

                var html = LabelHelper.GetTranslation("ResetPassword.CheckMailbox", true);

                if (status != null)
                    html = html.Replace("$FromEmail", $"{status.SenderName} &#60;{status.SenderEmail}&#62;");

                StatusAlert.AddMessage(AlertType.Success, html);
                ScreenViews.Visible = false;
            }
            catch (Exception)
            {
                try
                {
                    ResetTokenFile.Delete(tokenId);
                }
                catch (Exception ex)
                {
                    AppSentry.SentryError(ex);
                }

                throw;
            }
        }

        private void ChangeButton_Click(object sender, EventArgs e)
        {
            if (Password.Text != PasswordConfirm.Text)
            {
                StatusAlert.AddMessage(AlertType.Error, Translate("New password and confirmation does not match."));
                return;
            }

            var tokenFile = TokenIdentifier.HasValue
                ? ResetTokenFile.Read(TokenIdentifier.Value)
                : null;

            if (tokenFile == null)
            {
                ShowStatus(AlertType.Error, "ResetPassword.InvalidLink", ResetTokenFile.ResetTokenFileExpirationTime);
                return;
            }

            var password = Password.Text;
            var exceptionSource = typeof(ResetPassword).FullName + "." + nameof(ChangeButton_Click);

            var contact = UserSearch.Select(tokenFile.UserIdentifier, x => x.Persons.Select(y => y.Organization));
            if (!contact.IsNewPasswordValid(password))
            {
                ShowStatus(AlertType.Error, "ResetPassword.NewPasswordSameAsOld");

                Password.Text = string.Empty;
                PasswordConfirm.Text = string.Empty;

                return;
            }

            if (!PasswordStrength.Validate())
            {
                ShowStatus(AlertType.Error, PasswordStrength.ValidationError);
                return;
            }

            tokenFile.Delete();

            if (!tokenFile.IsValid(out var validationError))
            {
                AppSentry.SentryWarning(validationError, exceptionSource);
                ShowStatus(AlertType.Error, "ResetPassword.InvalidLink", ResetTokenFile.ResetTokenFileExpirationTime);
                return;
            }

            var user = ServiceLocator.UserSearch.GetUser(contact.UserIdentifier);
            user.SetPassword(password);
            UserStore.Update(user, null);

            StatusAlert.AddMessage(AlertType.Success, "far fa-check fs-xl", LabelHelper.GetTranslation("ResetPassword.ChangeSuccess", true));
            ScreenViews.Visible = false;

            AccountHelper.OnPasswordChanged(Organization.Key, contact.UserIdentifier, Request.UserHostAddress);
        }

        #endregion

        #region Methods (views)

        private void ShowError(string errorName, string extraText)
        {
            var text = GetDisplayText(errorName, null) + extraText;
            StatusAlert.AddMessage(AlertType.Error, text);
        }

        private void ShowStatus(AlertType type, string identifier, params object[] args)
        {
            var text = GetDisplayText(identifier, null);
            if (args.Length > 0)
                text = string.Format(text, args);

            StatusAlert.AddMessage(type, text);
        }

        #endregion

        #region Methods (redirect)

        public static void Redirect(bool endResponse = true)
        {
            var url = GetUrl();

            HttpResponseHelper.Redirect(url, endResponse);
        }

        public static string GetUrl() => "/ui/lobby/reset-password";

        #endregion

        #region Methods (helpers)

        private object GetSessionValue([CallerMemberName] string name = null)
        {
            return string.IsNullOrEmpty(name)
                ? null
                : Session[typeof(InSite.UI.Layout.Lobby.Controls.SignInBasePage) + "." + name];
        }

        private void SetSessionValue(object value, [CallerMemberName] string name = null)
        {
            if (!string.IsNullOrEmpty(name))
                Session[typeof(InSite.UI.Layout.Lobby.Controls.SignInBasePage) + "." + name] = value;
        }

        #endregion
    }
}