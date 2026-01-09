using System;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;

using InSite.Common.Web;
using InSite.Domain.Messages;
using InSite.Persistence;
using InSite.UI.Lobby;
using InSite.UI.Lobby.Utilities;
using InSite.Web.Security;
using InSite.Web.SignIn;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Layout.Lobby.Controls
{
    public partial class SignInBasePage : LobbyBasePage
    {
        #region Properties

        protected string RedirectURL
        {
            get => (string)GetSessionValue();
            set => SetSessionValue(value);
        }

        protected string AuthenticationSource
        {
            get => (string)GetSessionValue();
            set => SetSessionValue(value);
        }

        public bool HasExternalReturnUrl
        {
            get => (bool?)GetSessionValue() ?? false;
            set => SetSessionValue(value);
        }

        protected SignInUserMFA MFA
        {
            get => (SignInUserMFA)GetSessionValue();
            set => SetSessionValue(value);
        }

        protected string ConfirmationCodeSession
        {
            get => GetSessionValue()?.ToString();
            set => SetSessionValue(value);
        }

        protected string CellNumber
        {
            get => GetSessionValue()?.ToString();
            set => SetSessionValue(value);
        }

        protected static Guid UserId
        {
            get => (Guid?)GetSessionValue() ?? Guid.Empty;
            set => SetSessionValue(value);
        }

        protected static bool IsAuthentication
        {
            get => (bool?)GetSessionValue() ?? false;
            set => SetSessionValue(value);
        }

        protected static bool SuccessfulSignInHappened
        {
            get => (bool?)GetSessionValue() ?? false;
            set => SetSessionValue(value);
        }

        protected string FinalSignInUrl
        {
            get => GetSessionValue()?.ToString() ?? "";
            set => SetSessionValue(value);
        }

        protected bool ErrorHappened
        {
            get => (bool?)GetSessionValue() ?? false;
            set => SetSessionValue(value);
        }

        protected void ClearSessionCache()
        {
            MFA = null;
            ConfirmationCodeSession = "";
            CellNumber = "";
            UserId = Guid.Empty;
            IsAuthentication = false;
            SuccessfulSignInHappened = false;
            RedirectURL = "";
        }

        #endregion

        #region Handlers

        private void OnAuthenticationError(string username, User user, AuthenticationResult authResult, string authSource)
        {
            if (user != null)
                AccountHelper.WriteLoginEvent(authResult, user, Organization.Identifier, authSource);

            AccountHelper.OnSignInFailedAttempt();

            var lockedUntil = AccountHelper.SignInLockedUntil;
            if (lockedUntil.HasValue)
            {
                OnBruteForceDetected(username, lockedUntil.Value);
                return;
            }

            var attempts = AccountHelper.SignInFailedAttemptsCount;
            var maxattempts = AccountHelper.MaxSignInAttemptsNumber;
            var error = AccountHelper.GetError(authResult);
            error += $"<br/><small>You have made {attempts} attempts to sign in, and you have {maxattempts - attempts} more attempts before your browser is locked out.</small>";
            error += $"<br/><small>If you have forgotten your password, then please click this <a href=\"{ResetPassword.GetUrl()}\">link</a> to reset your password.</small>";

            if (authResult != AuthenticationResult.Unapproved)
                RedirectToSignInErrorPage(SignInErrorCodes.WrongUserNamePassword, false, error);

            if (username.IsNotEmpty())
                error += $"<br/>You can <a href='/ui/lobby/request-access?ticket={RequestAccess.CreateTicket(username)}'>request access here</a>.";

            RedirectToSignInErrorPage(SignInErrorCodes.UnApproved, false, error);
        }

        protected void OnBruteForceDetected(string username, DateTime lockedUntil)
        {
            ServiceLocator.AlertMailer.Send(new AuthenticationAlarmTriggeredNotification
            {
                OriginOrganization = Organization.Identifier,
                OriginUser = Shift.Constant.UserIdentifiers.Someone,

                FailedLoginCount = AccountHelper.SignInFailedAttemptsCount,
                Organization = Organization.LegalName,
                SignInUrl = Request.RawUrl,
                UserEmail = username,
                UserHostAddress = Request.UserHostAddress
            });

            RedirectToSignInErrorPage(SignInErrorCodes.BruteForce, false, AccountHelper.GetBruteForceError("Login", lockedUntil));
        }

        #endregion

        #region Methods (SignIn)

        protected void LoginUser(string username, string password, bool autologin, string authSource, string returnVerifiedUrl = null)
        {
            var (authResult, user) = SignInLogic.AuthenticateUser(username, password, autologin);

            if (authResult != AuthenticationResult.Success)
            {
                OnAuthenticationError(username, user, authResult, authSource);
                return;
            }

            var (mfaEnabled, mfa) = SignInLogic.CheckUserMFAStatus(user.UserIdentifier);
            if (mfaEnabled)
                RedirectToMFAPage(user.UserIdentifier, mfa);
            else
            {
                if (returnVerifiedUrl.HasValue())
                {
                    RedirectURL = RedirectReturnVerifiedUrl(user, returnVerifiedUrl);
                    HasExternalReturnUrl = RedirectURL.HasValue();
                }

                AuthenticationSource = authSource;

                RedirectToSignInSucceedPage(true, user.UserIdentifier);
            }
        }

        #endregion

        #region Methods (Redirect)

        public static string RedirectReturnVerifiedUrl(User user, string returnBackUrl)
        {
            var person = PersonSearch.Select(Organization.Identifier, user.UserIdentifier);
            if (person == null)
            {
                ServiceLocator.Logger.Information($"Person not found userId: {user.UserIdentifier} orgId: {Organization.Identifier}");
                return null;
            }

            var secret = TokenHelper.GetClientSecret(person.PersonIdentifier, false);

            var decodedReturnVerifiedUrl = TokenHelper.DecodeReturnBackUrl(returnBackUrl);

            if (decodedReturnVerifiedUrl.HasNoValue())
                return null;

            var tokenRequest = new Shift.Common.JwtRequest
            {
                Secret = secret.SecretValue,
                Organization = Organization?.Identifier
            };

            string token = TokenHelper.GenerateToken(tokenRequest, HttpContext.Current.Request.Url.ToString());

            if (token == null)
            {
                ServiceLocator.Logger.Information($"Token not generated for userId: {user.UserIdentifier} orgId: {Organization.Identifier} secret: {secret.SecretValue}");
                token = "INVALID-TOKEN";
            }

            return $"{decodedReturnVerifiedUrl}?token={token}";
        }

        protected void RedirectToParent()
        {
            var returnUrl = HttpContext.Current.Request.QueryString["returnUrl"];
            if (string.IsNullOrWhiteSpace(returnUrl)) returnUrl = SignInLogic.SignInPageURL;
            HttpResponseHelper.Redirect(returnUrl, true);
        }

        protected string MFAPageURL(Guid userId, TUserAuthenticationFactor mfa)
        {
            UserId = userId;
            mfa.User = null;
            MFA = new SignInUserMFA(mfa);
            var returnUrl = GetReturnURL();
            return $"{SignInLogic.SignInMFAPageURL}?returnUrl={returnUrl}";
        }

        protected void RedirectToMFAPage(Guid userId, TUserAuthenticationFactor mfa)
        {
            HttpResponseHelper.Redirect(MFAPageURL(userId, mfa), true);
        }

        public static string SignInSucceedPageURL(bool isAuthentication, Guid userId, string message = "")
        {
            SuccessfulSignInHappened = true;
            UserId = userId;
            IsAuthentication = isAuthentication;

            if (!string.IsNullOrWhiteSpace(message))
                return $"{SignInLogic.SignInChallengePageURL}?message={UrlEncoder.Default.Encode(message)}";
            else
                return SignInLogic.SignInChallengePageURL;
        }

        public static void RedirectToSignInSucceedPage(bool isAuthentication, Guid userId, string message = "")
        {
            HttpResponseHelper.Redirect(SignInSucceedPageURL(isAuthentication, userId, message), true);
        }

        protected void RedirectToSignInErrorPage(SignInErrorCodes errorCode, bool htmlEncode = false, string message = "")
        {
            ErrorHappened = true;

            var returnUrl = GetReturnURL();
            var redirectUrl = !string.IsNullOrWhiteSpace(message)
                ? $"{SignInLogic.SignInErrorPageURL}?returnUrl={returnUrl}&message={UrlEncoder.Default.Encode(message)}&htmlEncode={htmlEncode}"
                : $"{SignInLogic.SignInErrorPageURL}?returnUrl={returnUrl}";

            HttpResponseHelper.Redirect(redirectUrl, true);
        }

        protected void RedirectToSignInPage(string message = "")
        {
            if (!string.IsNullOrWhiteSpace(message)) HttpResponseHelper.Redirect($"{SignInLogic.SignInPageURL}?message={UrlEncoder.Default.Encode(message)}", true);
            else HttpResponseHelper.Redirect(SignInLogic.SignInPageURL, true);
        }

        #endregion

        #region Methods (helpers)

        private static object GetSessionValue([CallerMemberName] string name = null)
        {
            return string.IsNullOrEmpty(name)
                ? null
                : HttpContext.Current.Session[typeof(SignInBasePage) + "." + name];
        }

        private static void SetSessionValue(object value, [CallerMemberName] string name = null)
        {
            if (!string.IsNullOrEmpty(name))
                HttpContext.Current.Session[typeof(SignInBasePage) + "." + name] = value;
        }

        private static readonly Regex _returnUrlReplacePattern = new Regex(
            "(/)(signin-challenge|signin-failure|signin-social|signin-success)($|\\?)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string GetReturnURL()
        {
            var returnUrl = _returnUrlReplacePattern.Replace(Request.Url.ToString(), "$1signin$3");

            return UrlEncoder.Default.Encode(returnUrl);
        }

        #endregion
    }
}