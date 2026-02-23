using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Security.Impersonation
{
    public partial class Stop : PortalBasePage
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

            var returnUrl = StopImpersonation();

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
                return Urls.HomeUrl;

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