using System;

using InSite.Common.Web;
using InSite.UI.Layout.Lobby.Controls;
using InSite.UI.Lobby.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Lobby.SignInPages
{
    public partial class SignInSucess : SignInBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var referrer = Page.Request.UrlReferrer;

            var signInUrl = FinalSignInUrl;

            FinalSignInUrl = null;

            if (signInUrl.IsEmpty())
                SignOut.Redirect(this, "Missing sign-in URL");

            else if (!EnsureAuthenticatedPersonHasSecret(out string error))
                SignOut.Redirect(this, error);

            else
                HttpResponseHelper.Redirect(signInUrl, true);
        }

        private bool EnsureAuthenticatedPersonHasSecret(out string error)
        {
            error = string.Empty;

            try
            {
                var user = Identity.User;

                if (user == null || user.Identifier == Guid.Empty || user.Identifier == UserIdentifiers.Someone)
                {
                    error = user == null
                        ? "User is null."
                        : user.Identifier == Guid.Empty
                        ? "User identifier is empty."
                        : "User is someone unknown.";

                    return false;
                }

                var organization = Identity.Organization;

                if (organization == null || organization.Identifier == Guid.Empty)
                {
                    error = organization == null
                        ? "Organization is null."
                        : "Organization identifier is empty.";

                    return false;
                }

                var person = ServiceLocator.PersonSearch.GetPerson(user.Identifier, organization.Identifier);

                var personId = person.PersonIdentifier;

                TokenHelper.GetClientSecret(personId, false);

                return true;
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);

                error = ex.Message;

                return false;
            }
        }
    }
}