using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Records.Read;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Progress")]
    [RoutePrefix("api/records/achievements/documents")]
    public class AchievementDocumentsController : ApiBaseController
    {
        /// <summary>
        /// List achievement documents
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of the achievements assigned to learners in your organization.
        /// </remarks>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            var list = GetDocumentList();
            return JsonSuccess(list);
        }

        /// <summary>
        /// Get an achievement document
        /// </summary>
        /// <remarks>
        /// Returns an achievement document using the document's globally unique identifier.
        /// </remarks>
        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage Get(string id)
        {
            var item = Guid.TryParse(id, out Guid guid)
                ? GetDocumentItem(guid)
                : null;

            return item != null
                ? JsonSuccess(item)
                : JsonError($"Achievement Template Not Found: {id}", System.Net.HttpStatusCode.BadRequest);
        }

        private Document[] GetDocumentList()
        {
            var organization = GetOrganization();

            var filter = new VCredentialFilter { OrganizationIdentifier = organization.Identifier };

            var credentials = ServiceLocator.AchievementSearch.GetCredentials(filter);
            return credentials.Select(
                x => new Document
                {
                    Identifier = x.CredentialIdentifier,
                    Template = x.AchievementIdentifier,
                    Status = x.CredentialStatus,
                    Granted = x.CredentialGranted.ToJsTime(),
                    Score = (int?)(x.CredentialGrantedScore * 100),
                    Expiry = x.CredentialExpirationExpected.ToJsTime(),
                    Learner = new Learner
                    {
                        Identifier = x.UserIdentifier,
                        Email = x.UserEmail,
                        Name = new PersonName
                        {
                            First = x.UserFirstName,
                            Last = x.UserLastName
                        }
                    },
                    CourseName = x.AchievementTitle
                })
                .Take(100)
                .ToArray();
        }

        private Document GetDocumentItem(Guid id)
        {
            var x = ServiceLocator.AchievementSearch.GetCredential(id);
            var item = new Document
            {
                Identifier = x.CredentialIdentifier,
                Template = x.AchievementIdentifier,
                Status = x.CredentialStatus,
                Granted = x.CredentialGranted.ToJsTime(),
                Score = (int?)(x.CredentialGrantedScore * 100),
                Expiry = x.CredentialExpirationExpected.ToJsTime(),
                Learner = new Learner
                {
                    Identifier = x.UserIdentifier,
                    Email = x.UserEmail,
                    Name = new PersonName
                    {
                        First = x.UserFirstName,
                        Last = x.UserLastName
                    },
                },
                CourseName = x.AchievementTitle
            };
            return item;
        }
    }
}