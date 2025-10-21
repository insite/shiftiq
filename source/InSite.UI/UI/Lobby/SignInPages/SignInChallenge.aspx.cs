using System;
using System.Linq;
using System.Text.Encodings.Web;

using InSite.Application.People.Write;
using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Lobby.Controls;
using InSite.Web.Security;
using InSite.Web.SignIn;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby.SignInPages
{
    public partial class SignInChallenge : SignInBasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Response.Headers.Add("X-Successful-Login-Attempt", false.ToString());

            ProcessRedirect(IsAuthentication);
        }

        private void ProcessRedirect(bool isAuthentication)
        {
            if (!SuccessfulSignInHappened)
                SignOut.Redirect(this, "Sign in failed");

            var user = UserId != Guid.Empty ? UserSearch.Select(UserId, x => x.Memberships.Select(y => y.Group)) : null;

            if (user == null)
                SignOut.Redirect(this, $"Authentication succeeded but user ID has no matching account: {UserId}");

            var result = SignInLogic.GetUserOrganization(user, isAuthentication, RedirectURL, HasExternalReturnUrl);

            if (result.Error != default)
                RedirectToSignInErrorPage(user, result.Error.Code, result.Error.HtmlEncode, result.Error.Message);

            if (result.Organization == null)
                SignOut.Redirect(this, $"Authentication succeeded but organization account is not assigned");

            RedirectToSuccessPage(isAuthentication, user, result.Organization, result.RedirectUrl);
        }

        private void RedirectToSuccessPage(bool isAuthentication, User user, OrganizationState organization, string redirectUrl)
        {
            try
            {
                Response.Headers["X-Successful-Login-Attempt"] = SuccessfulSignInHappened.ToString();

                ClearSessionCache();

                FinalSignInUrl = GetStaticRedirectFromLoginPageURL(user, isAuthentication, organization, redirectUrl);
            }
            catch (MissingPersonException mpex)
            {
                CurrentIdentityFactory.SignedOut();

                RedirectToSignInErrorPage(user, SignInErrorCodes.Unauthorized, true, mpex.Message);
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
                RedirectToSignInErrorPage(SignInErrorCodes.UnknownException, true, "An unexpected error occurred. Please try again later.");
            }

            var url = ServiceLocator.Urls.GetApplicationUrl(organization.OrganizationCode) + "/ui/lobby/signin-success";
            HttpResponseHelper.Redirect(url);
        }

        private void RedirectToSignInErrorPage(User user, SignInErrorCodes errorCode, bool htmlEncode = false, string message = "")
        {
            var isUnauthorized = errorCode == SignInErrorCodes.Unauthorized || errorCode == SignInErrorCodes.UnAuthorizedForOrganization;

            if (isUnauthorized && user.Email.IsNotEmpty())
            {
                if (htmlEncode)
                {
                    htmlEncode = false;
                    message = HtmlEncoder.Default.Encode(message);
                }

                message += $"<br/>You can <a href='/ui/lobby/request-access?ticket={RequestAccess.CreateTicket(user.Email)}'>request access here</a>.";
            }

            RedirectToSignInErrorPage(errorCode, htmlEncode, message);
        }

        private string GetStaticRedirectFromLoginPageURL(User user, bool isAuthentication, OrganizationState organization, string redirectUrl)
        {
            if (!isAuthentication)
                return redirectUrl;

            var impersonatorUser = CurrentSessionState.Identity.Impersonator?.User?.Email;
            var impersonatorOrganization = CurrentSessionState.Identity.Impersonator?.Organization?.Code;
            var person = PersonSearch.Select(organization.Identifier, user.UserIdentifier);
            var authSource = AuthenticationSource.IfNullOrEmpty(Shift.Constant.AuthenticationSource.None);

            CurrentIdentityFactory.SignedIn(
                user.Email,
                user.UserIdentifier,
                organization.OrganizationCode,
                impersonatorOrganization,
                impersonatorUser,
                person?.Language ?? CookieTokenModule.Current.Language,
                user.TimeZone,
                authSource);

            CurrentSessionState.DateSignedIn = DateTime.UtcNow;

            SessionHelper.StartSession(Organization.OrganizationIdentifier, user.UserIdentifier);

            ServiceLocator.SendCommand(new ModifyPersonFieldDateOffset(person.PersonIdentifier, Domain.Contacts.PersonField.LastAuthenticated, DateTimeOffset.UtcNow));

            AccountHelper.WriteLoginEvent(AuthenticationResult.Success, user, Organization.OrganizationIdentifier, authSource);

            RecentSessionHelper.Clear();

            return redirectUrl;
        }
    }
}