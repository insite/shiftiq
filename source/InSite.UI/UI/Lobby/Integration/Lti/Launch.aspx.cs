using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Lobby.Integration.Lti
{
    public partial class Launch : Page
    {
        private bool Debug => bool.TryParse(Request["debug"], out var debug) && debug;

        private string CurrentOrganization => CookieTokenModule.Current.OrganizationCode;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var errors = new List<string>();

            var model = new LaunchProviderModel(Request.Form, Debug);

            var email = Request.Form["lis_person_contact_email_primary"];

            if (!email.HasValue())
                HttpResponseHelper.SendHttp400();

            if (Debug)
            {
                var sortedParameters = new SortedDictionary<string, string>();
                foreach (var key in Request.Form.AllKeys)
                    sortedParameters.Add(key, Request.Form[key]);

                var parameterItems = sortedParameters.Select(x => new { x.Key, x.Value });

                ParameterRepeater.DataSource = parameterItems;
                ParameterRepeater.DataBind();

                return;
            }

            var organization = OrganizationSearch.Select(CurrentOrganization);

            var user = Authenticate(organization, email, errors);

            var signature = model.GetSignature(organization.OrganizationIdentifier, "POST", Request.Url);

            if (signature != Request.Form["oauth_signature"])
                errors.Add("OAuth Signature Mismatch");

            ErrorRepeater.DataSource = errors;
            ErrorRepeater.DataBind();

            if (user != null)
            {
                var groupName = Request.Form["custom_group_name"];

                if (groupName != null)
                {
                    // We can do something with the group name here.
                }

                HttpResponseHelper.Redirect(ServiceLocator.Urls.RootUrl);
            }
        }

        private QUser Authenticate(OrganizationState organization, string email, List<string> errors)
        {
            var organizationSecret = Request.Form["oauth_consumer_key"];

            if (!StringHelper.Equals(organizationSecret, organization.OrganizationSecret))
            {
                errors.Add($"OAuth Validation Failed: Organization Secret Mismatch");
                return null;
            }

            var organizationIdentifier = Request.Form["shift_organization_identifier"];

            if (Guid.TryParse(organizationIdentifier, out Guid result))
            {
                if (organization.OrganizationIdentifier != result)
                {
                    errors.Add($"OAuth Validation Failed: Organization Identifier Mismatch");
                    return null;
                }
            }

            var lastName = Request.Form["lis_person_name_family"];
            var firstName = Request.Form["lis_person_name_given"];
            var code = Request.Form["user_id"];

            var user = GetOrCreateUser(firstName, lastName, email, code, organization, errors);
            if (user == null)
                return null;

            CurrentIdentityFactory.SignedIn(email, user.UserIdentifier, CurrentOrganization, null, null, "en", user.TimeZone, AuthenticationSource.LtiLaunch);

            var identityOrganization = CurrentSessionState.Identity?.Organization?.Code;
            if (identityOrganization != CurrentOrganization)
                throw new Exception($"Organization Mismatch: {identityOrganization ?? "null"} <> {CurrentOrganization}");

            CurrentSessionState.DateSignedIn = DateTime.UtcNow;

            SessionHelper.StartSession(organization.OrganizationIdentifier, user.UserIdentifier);

            RecentSessionHelper.Clear();

            return user;
        }

        private QUser GetOrCreateUser(string firstName, string lastName, string userEmail, string personCode, OrganizationState organization, List<string> errors)
        {
            var user = ServiceLocator.UserSearch.GetUserByEmail(userEmail);
            if (user != null)
                return user;

            if (!string.IsNullOrEmpty(personCode)
                && ServiceLocator.PersonSearch.IsPersonExist(organization.Identifier, personCode)
                )
            {
                errors.Add($"The person with the code '{personCode}' already exists");
                return null;
            }

            return CreateNewUser(firstName, lastName, userEmail, personCode, organization);
        }

        private QUser CreateNewUser(string firstName, string lastName, string userEmail, string personCode, OrganizationState organization)
        {
            var user = UserFactory.Create();

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = userEmail;
            user.SetDefaultPassword();

            var person = new QPerson();

            person.UserIdentifier = user.UserIdentifier;
            person.OrganizationIdentifier = organization.Identifier;
            person.IsLearner = true;
            person.PersonCode = personCode;
            person.EmailEnabled = true;
            person.UserAccessGranted = DateTimeOffset.UtcNow;
            person.UserAccessGrantedBy = "Launch Lti";

            UserStore.Insert(user, person);

            MembershipHelper.Save(organization.OrganizationIdentifier, "Role", organization.CompanyName + " Learner", user.UserIdentifier, "Membership");

            return user;
        }
    }

    public class LaunchProviderModel
    {
        public NameValueCollection Parameters { get; set; }

        public bool EnableDebugging { get; set; }

        public string GetSignature(Guid organization, string method, Uri uri)
        {
            return LtiTicketHelper.GenerateSignature(organization.ToString().Substring(24, 12).ToUpper(), method, uri, Parameters);
        }

        public SortedDictionary<string, string> SortedParameters
        {
            get
            {
                var parameters = new SortedDictionary<string, string>();
                foreach (var key in Parameters.AllKeys)
                    parameters.Add(key, Parameters[key]);
                return parameters;
            }
        }

        public LaunchProviderModel(NameValueCollection parameters, bool debug)
        {
            EnableDebugging = debug;
            Parameters = parameters;
        }
    }
}