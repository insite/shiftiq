using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Lobby;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Accounts.Users
{
    public partial class Impersonate : PortalBasePage
    {
        private static string ReturnUrl
        {
            get
            {
                var returnUrl = HttpContext.Current.Request["ReturnUrl"];
                return returnUrl.HasValue()
                    ? returnUrl
                    : ServiceLocator.Urls.GetHomeUrl(Identity.User.AccessGrantedToCmds, ServiceLocator.Partition.IsE03(), Identity.IsAdministrator);
            }
        }

        private string OrganizationUrl => PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, Identity.Organization.OrganizationCode, ReturnUrl);

        private Guid? UserIdentifier => Guid.TryParse(Page.Request.QueryString["user"], out var value) ? value : (Guid?)null;

        private class UserAndPerson
        {
            public User User { get; }
            public QPerson Person { get; }
            public List<QPerson> AllPersons { get; }

            public UserAndPerson(User user, QPerson person, List<QPerson> allPersons)
            {
                User = user;
                Person = person;
                AllPersons = allPersons;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var isStart = UserIdentifier.HasValue && UserSearch.WebContactExist(UserIdentifier.Value, Organization.Identifier);

            var returnUrl = isStart
                ? StartImpersonation(UserIdentifier.Value)
                : StopImpersonation();

            if (returnUrl != null && returnUrl != Page.Request.RawUrl)
            {
                ContinueButton.NavigateUrl = returnUrl;
                ContinueButton.Visible = true;
                HttpResponseHelper.Redirect(returnUrl);
            }

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
        }

        private string StartImpersonation(Guid user)
        {
            var impersonator = GetUser(Identity.User.UserIdentifier);
            var impersonated = GetUser(user);

            if (impersonator.User == null)
                HttpResponseHelper.Redirect(SignOut.GetUrl());

            if (impersonated.User.IsCloaked && !impersonator.User.IsCloaked)
                return ReturnUrl;

            DetailUserToImpersonate.Text = $"<strong>{impersonated.User.FullName}</strong><br />{impersonated.User.Email}";
            DetailAdministratorName.Text = $"<strong>{impersonator.User.FullName}</strong><br />{impersonator.User.Email}";
            DetailAdministratorGroups.DataSource = Identity.Groups;
            DetailAdministratorGroups.DataBind();

            if (!Identity.IsImpersonating)
            {
                if (!AllowImpersonation(impersonator, impersonated, Identity.Organization))
                    return ImpersonatorCannotSwitchOrganizations(impersonator, impersonated, Identity.Organization);

                CurrentSessionModule.Initialize(HttpContext.Current, Identity);

                ClearSessionVariables();

                Identity.User = UserAdapter.ToModel(impersonated.User, impersonated.Person.PersonCode, impersonated.Person.Phone, impersonated.Person.JobTitle, null);
                Identity.Impersonator = new Domain.Foundations.Impersonator();
                Identity.Impersonator.User = UserAdapter.ToModel(impersonator.User, impersonator.Person.PersonCode, impersonator.Person.Phone, impersonator.Person.JobTitle, null);
                Identity.Impersonator.Organization = OrganizationSearch.Select(Identity.Organization.Identifier);
                Identity.Impersonator.Organizations = Identity.Organizations;

                ImpersonationStore.Start(impersonated.User.UserIdentifier, impersonator.User.UserIdentifier);

                try
                {
                    CurrentIdentityFactory.SignedIn(
                        impersonated.User.Email,
                        impersonated.User.UserIdentifier,
                        Identity.Organization.OrganizationCode,
                        Identity.Impersonator.Organization.Code,
                        Identity.Impersonator.User.Email,
                        impersonated?.Person?.Language,
                        impersonated?.User?.TimeZone,
                        CookieTokenModule.Current.AuthenticationSource
                    );
                }
                catch (MissingPersonException)
                {
                    var source = typeof(Impersonate).FullName + "." + nameof(StartImpersonation);

                    var error = $"{impersonator.User.Email} cannot impersonate {impersonated.User.Email}. " +
                        $"Both {impersonator.User.Email} and {impersonated.User.Email} must be granted access to organization account '{Identity.Organization?.OrganizationCode}' before this impersonation is permitted.";

                    AppSentry.SentryWarning(error, source);

                    CurrentIdentityFactory.SignedOut();

                    HttpResponseHelper.Redirect(ServiceLocator.Urls.LoginUrl);
                }

                return OrganizationUrl;
            }
            else
            {
                return ImpersonatorCannotImpersonate(Identity.User.Email);
            }
        }

        private bool AllowImpersonation(UserAndPerson impersonator, UserAndPerson impersonated, OrganizationState organization)
        {
            var isAdministrator = impersonator.Person != null;
            var isUser = impersonated.Person != null;
            var isAuthorized = Identity.IsGranted("ui/portal/identity/impersonate");

            if (!isAdministrator)
                ImpersonationStatus.AddMessage(AlertType.Error, $"{impersonator.User.FullName} {Translate("is not connected to")} {organization.CompanyName}.");

            if (!isUser)
                ImpersonationStatus.AddMessage(AlertType.Error, $"{impersonated.User.FullName} {Translate("is not connected to")} {organization.CompanyName}.");

            if (!isAuthorized)
                ImpersonationStatus.AddMessage(AlertType.Error, $"{impersonator.User.FullName} {Translate("is not assigned to a role with access granted to")} ui/portal/identity/impersonate.");

            return isAdministrator && isUser && isAuthorized;
        }

        private string ImpersonatorCannotSwitchOrganizations(UserAndPerson impersonator, UserAndPerson impersonated, OrganizationState organization)
        {
            // Validate parameters.

            if (impersonator == null)
                throw new ArgumentNullException(nameof(impersonator));

            if (impersonator.AllPersons == null)
                throw new ArgumentNullException(nameof(impersonator.AllPersons));

            if (impersonator.User == null)
                throw new ArgumentNullException(nameof(impersonator.User));

            if (impersonated == null)
                throw new ArgumentNullException(nameof(impersonated));

            if (impersonated.User == null)
                throw new ArgumentNullException(nameof(impersonated.User));

            if (organization == null)
                throw new ArgumentNullException(nameof(organization));

            // Build a user-friendly error message.

            var html = new StringBuilder();

            html.Append($@"<p>{Translate("Your account")} <strong>({impersonator.User.Email})</strong>
                        {Translate("does not have permission to impersonate")} <strong>({impersonated.User.Email})</strong>
                        {Translate("while you have")} <strong>{organization.CompanyName}</strong> {Translate("selected for your session")}.</p>");

            var impersonatorOrganizations = impersonator.AllPersons
                .Where(x => x.IsAdministrator && x.Organization.AccountClosed == null)
                .Select(x => x.Organization)
                .OrderBy(x => x.CompanyName);

            var impersonatedOrganizations = impersonated.AllPersons
                .Where(x => (x.IsAdministrator || x.IsLearner) && x.Organization.AccountClosed == null)
                .Select(x => x.Organization)
                .OrderBy(x => x.CompanyName)
                .ToArray();

            if (impersonatorOrganizations.Count() > 0)
            {
                if (impersonatedOrganizations.Length > 0)
                {
                    html.Append($"<p>{Translate("Please select one of the following organizations before impersonating a user")}:</p><ul>");
                    foreach (var impersonatedOrganization in impersonatedOrganizations)
                        html.Append($"<li>{impersonatedOrganization.CompanyName}</li>");
                    html.Append("</ul>");
                }
                else
                {
                    html.Append($"<p>{impersonated.User.FullName} {Translate("is not granted access to any organization account")}.</p><ul>");
                }
            }
            else
            {
                html.Append($@"<p>{Translate("Please contact your system administrator to grant the necessary permissions to your account")}.</p>");
            }

            ImpersonationStatus.AddMessage(AlertType.Error, html.ToString());
            return null;
        }

        private string ImpersonatorCannotImpersonate(string login)
        {
            var error = $"{Translate("You must stop impersonating")} {login} {Translate("before you start impersonating another user account")}.";
            ImpersonationStatus.AddMessage(AlertType.Error, error);
            StopButton.Visible = true;
            return null;
        }

        private string StopImpersonation()
        {
            if (!Identity.IsImpersonating)
                return ReturnUrl;

            var userIdentifier = Identity.User.UserIdentifier;

            ImpersonationStore.Stop(Identity.Impersonator.User.UserIdentifier);

            var impersonator = GetUser(Identity.Impersonator.User.UserIdentifier);

            Identity.User = UserAdapter.ToModel(impersonator.User, impersonator.Person.PersonCode, impersonator.Person.Phone, impersonator.Person.JobTitle, null);
            Identity.Impersonator = null;

            CurrentSessionModule.Initialize(HttpContext.Current, Identity);

            ClearSessionVariables();

            if (Identity?.Impersonator?.Organization != null)
            {
                CurrentIdentityFactory.SignedIn(impersonator.User.Email, impersonator.User.UserIdentifier, Identity.Impersonator.Organization.Code, null, null, impersonator.Person?.Language, impersonator.User?.TimeZone, CookieTokenModule.Current.AuthenticationSource);
            }

            if (ServiceLocator.Partition.IsE03())
                return Urls.CmdsHomeUrl;

            return $"/ui/admin/contacts/people/edit?contact={userIdentifier}";
        }

        private void ClearSessionVariables()
        {
            HttpContext.Current.Session[SessionKeys.GetImpersonatorPermissions] = null;
            HttpContext.Current.Session[SessionKeys.GetUserPermissions] = null;
        }

        private UserAndPerson GetUser(Guid userId)
        {
            var user = UserSearch.Select(userId);
            var allPersons = ServiceLocator.PersonSearch.GetPersons(new QPersonFilter { UserIdentifier = userId }, x => x.Organization);
            var person = allPersons.FirstOrDefault(x => x.OrganizationIdentifier == Organization.Identifier);

            return new UserAndPerson(user, person, allPersons);
        }
    }
}