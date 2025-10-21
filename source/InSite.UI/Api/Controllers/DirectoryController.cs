using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Contacts.Read;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Api.Controllers
{
    [DisplayName("Directory")]
    [RoutePrefix("api/learners")]
    public class LearnersController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Returns a list of all people in your organization, including names, email addresses, and home addresses.
        /// </remarks>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            var list = GetLearnerList(null);
            return JsonSuccess(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Returns a person using the person's unique globally unique identifier or individual account code. Account codes are alphanumeric values 
        /// assigned by your organization.
        /// </remarks>
        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage Get(string id)
        {
            var item = Guid.TryParse(id, out Guid guid)
                ? GetLearnerItem(guid)
                : GetLearnerItem(id);

            return item != null
                ? JsonSuccess(item)
                : JsonError($"Learner Not Found: {id}", System.Net.HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Updates the person's name, email address, and home address. If the person is not in the database, then this method adds them as a new 
        /// person.
        /// </remarks>
        [HttpPost]
        [Route("save")]
        [Route("{id}/save")]
        public HttpResponseMessage Save(Models.Documents.Learner learner)
        {
            try
            {
                SaveLearner(learner);
                return JsonSuccess("OK");
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Counts the learners who match your search criteria. You can search based on any of the following property values: Code, Email, FirstName,
        /// and/or LastName.
        /// </remarks>
        [HttpPost]
        [Route("count")]
        [Route("{id}/count")]
        public HttpResponseMessage Count(Models.Documents.LearnerCriteria criteria)
        {
            try
            {
                return JsonSuccess(GetLearnerList(criteria).Length);
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Searches for learners who match your search criteria. You can search based on any of the following property values: Code, Email, 
        /// FirstName, and/or LastName.
        /// </remarks>
        [HttpPost]
        [Route("search")]
        [Route("{id}/search")]
        public HttpResponseMessage Search(Models.Documents.LearnerCriteria criteria)
        {
            try
            {
                return JsonSuccess(GetLearnerList(criteria));
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        private Models.Documents.Learner[] GetLearnerList(Models.Documents.LearnerCriteria criteria)
        {
            var organization = GetOrganization();
            var learners = PersonCriteria.Bind(
                x => new Models.Documents.Learner
                {
                    Identifier = x.UserIdentifier,
                    FirstName = x.User.FirstName,
                    MiddleName = x.User.MiddleName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Code = x.PersonCode,

                    IsAdministrator = x.IsAdministrator,
                    IsLearner = x.IsLearner,

                    HomeAddress = new Models.Documents.Address
                    {
                        Street1 = x.HomeAddress.Street1,
                        Street2 = x.HomeAddress.Street2,
                        City = x.HomeAddress.City,
                        State = x.HomeAddress.Province,
                        Country = x.HomeAddress.Country,
                        PostalCode = x.HomeAddress.PostalCode
                    }
                },
                new PersonFilter
                {
                    OrganizationIdentifier = organization.OrganizationIdentifier,
                    OrderBy = "FirstName,LastName"
                }
            );

            if (criteria != null)
            {
                var query = learners.AsQueryable();
                if (!string.IsNullOrEmpty(criteria.Code))
                    query = query.Where(x => x.Code.StartsWith(criteria.Code));
                if (!string.IsNullOrEmpty(criteria.Email))
                    query = query.Where(x => x.Email.StartsWith(criteria.Email));
                if (!string.IsNullOrEmpty(criteria.FirstName))
                    query = query.Where(x => x.FirstName.StartsWith(criteria.FirstName));
                if (!string.IsNullOrEmpty(criteria.LastName))
                    query = query.Where(x => x.LastName.StartsWith(criteria.LastName));
                learners = query.ToArray();
            }

            return learners;
        }

        private Models.Documents.Learner GetLearnerItem(Guid id)
        {
            var organization = GetOrganization();

            return PersonCriteria.BindFirst(
                x => new Models.Documents.Learner
                {
                    Identifier = x.UserIdentifier,
                    FirstName = x.User.FirstName,
                    MiddleName = x.User.MiddleName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Code = x.PersonCode,
                    HomeAddress = new Models.Documents.Address
                    {
                        Street1 = x.HomeAddress.Street1,
                        Street2 = x.HomeAddress.Street2,
                        City = x.HomeAddress.City,
                        State = x.HomeAddress.Province,
                        Country = x.HomeAddress.Country,
                        PostalCode = x.HomeAddress.PostalCode
                    }
                },
                new PersonFilter
                {
                    OrganizationIdentifier = organization.Identifier,
                    UserIdentifier = id
                });
        }

        private Models.Documents.Learner GetLearnerItem(string code)
        {
            var organization = GetOrganization();

            return PersonCriteria.BindFirst(
                x => new Models.Documents.Learner
                {
                    Identifier = x.UserIdentifier,
                    FirstName = x.User.FirstName,
                    MiddleName = x.User.MiddleName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Code = x.PersonCode,
                    HomeAddress = new Models.Documents.Address
                    {
                        Street1 = x.HomeAddress.Street1,
                        Street2 = x.HomeAddress.Street2,
                        City = x.HomeAddress.City,
                        State = x.HomeAddress.Province,
                        Country = x.HomeAddress.Country,
                        PostalCode = x.HomeAddress.PostalCode
                    }
                },
                new PersonFilter
                {
                    OrganizationIdentifier = organization.Identifier,
                    CodeContains = code
                });
        }

        private void SaveLearner(Models.Documents.Learner learner)
        {
            var organization = GetOrganization();

            if (learner.Identifier == Guid.Empty)
                throw new Exception("The learner is missing a unique identifier value. (Learner.Identifier is a required property.)");

            if (!ServiceLocator.PersonSearch.IsPersonExist(learner.Identifier, organization.Identifier))
                Insert(learner, organization.Identifier, organization.TimeZone.Id, organization.Toolkits.Contacts?.DefaultMFA ?? false);
            else
                Update(learner, organization.Identifier);
        }

        private void Insert(Models.Documents.Learner learner, Guid organization, string timezone, bool defaultMFA)
        {
            var user = new QUser
            {
                UserIdentifier = learner.Identifier,
                TimeZone = timezone,
                FirstName = learner.FirstName,
                MiddleName = learner.MiddleName,
                LastName = learner.LastName,
                Email = learner.Email,
                MultiFactorAuthentication = defaultMFA
            };

            var person = new QPerson
            {
                UserIdentifier = learner.Identifier,
                OrganizationIdentifier = organization,
                IsLearner = true,
                PersonCode = learner.Code,
                EmailEnabled = true
            };

            person.HomeAddress = new QPersonAddress
            {
                Street1 = learner.HomeAddress.Street1,
                Street2 = learner.HomeAddress.Street2,
                City = learner.HomeAddress.City,
                Province = learner.HomeAddress.State,
                Country = learner.HomeAddress.Country,
                PostalCode = learner.HomeAddress.PostalCode
            };

            UserStore.Insert(user, person);
        }

        private void Update(Models.Documents.Learner learner, Guid organization)
        {
            // The developer can update an existing person using the API only if:
            //   1 - The developer is an administrator in the organization account, and
            //   2 - The person is a learner in the organization account.

            if (!IsAdministrator)
                throw new Exception($"Your developer account is not an Administrator in the {CurrentOrganization.Name} organization account, therefore you cannot apply changes to Learners using the API.");

            if (!learner.IsLearner)
                throw new Exception($"{learner.FirstName} {learner.LastName} is not a Learner in the {CurrentOrganization.Name} organization account, therefore you cannot apply changes to this contact using the API.");

            var user = ServiceLocator.UserSearch.GetUser(learner.Identifier);

            user.FirstName = learner.FirstName;
            user.MiddleName = learner.MiddleName;
            user.LastName = learner.LastName;
            user.Email = learner.Email;

            if (!string.IsNullOrEmpty(learner.Password))
                user.UserPasswordHash = PasswordHash.CreateHash(learner.Password);

            var fullNamePolicy = OrganizationSearch.GetPersonFullNamePolicy(organization);

            UserStore.Update(user, fullNamePolicy);

            var p = ServiceLocator.PersonSearch.GetPerson(learner.Identifier, organization, x => x.HomeAddress);
            p.PersonCode = learner.Code;

            if (learner.AccessGranted.HasValue)
            {
                p.UserAccessGranted = learner.AccessGranted;
                p.UserAccessGrantedBy = CurrentUser.FullName;
            }

            if (learner.AccessRevoked.HasValue)
            {
                p.AccessRevoked = learner.AccessRevoked;
                p.AccessRevokedBy = CurrentUser.FullName;
            }

            if (p.HomeAddress == null)
                p.HomeAddress = new QPersonAddress();

            p.HomeAddress.Street1 = learner.HomeAddress.Street1;
            p.HomeAddress.Street2 = learner.HomeAddress.Street2;
            p.HomeAddress.City = learner.HomeAddress.City;
            p.HomeAddress.Province = learner.HomeAddress.State;
            p.HomeAddress.Country = learner.HomeAddress.Country;
            p.HomeAddress.PostalCode = learner.HomeAddress.PostalCode;

            PersonStore.Update(p);
        }
    }
}

namespace InSite.Api.Models
{
    namespace Documents
    {
        public class Learner
        {
            public Guid Identifier { get; set; }

            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }

            public string Email { get; set; }
            public string Code { get; set; }
            public string Password { get; set; }

            public bool IsAdministrator { get; set; }
            public bool IsLearner { get; set; }

            public DateTimeOffset? AccessGranted { get; set; }
            public DateTimeOffset? AccessRevoked { get; set; }

            public Address HomeAddress { get; set; }

            public Learner()
            {
                HomeAddress = new Address();
            }
        }

        public class Address
        {
            public string Street1 { get; set; }
            public string Street2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string PostalCode { get; set; }
        }

        public class LearnerCriteria
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Code { get; set; }
        }
    }
}