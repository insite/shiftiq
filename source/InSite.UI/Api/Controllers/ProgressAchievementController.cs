using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

namespace InSite.Api.Controllers
{
    [DisplayName("Progress")]
    [RoutePrefix("api/records/achievements/templates")]
    public class AchievementTemplatesController : ApiBaseController
    {
        /// <summary>
        /// List achievement templates
        /// </summary>
        /// <remarks>
        /// Use this request to query for a listing of all the achievements in your library.
        /// </remarks>
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            var list = GetTemplateList();
            return JsonSuccess(list);
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage Get(string id)
        {
            var item = Guid.TryParse(id, out Guid guid)
                ? GetTemplateItem(guid)
                : null;

            return item != null
                ? JsonSuccess(item)
                : JsonError($"Achievement Template Not Found: {id}", System.Net.HttpStatusCode.BadRequest);
        }

        private Models.Records.Template[] GetTemplateList()
        {
            var organization = GetOrganization();
            var filter = new QAchievementFilter(organization.OrganizationIdentifier);
            var list = ServiceLocator.AchievementSearch.GetAchievements(filter)
                .Select(x => new Models.Records.Template
                {
                    Identifier = x.AchievementIdentifier,
                    Name = x.AchievementTitle,
                    Label = x.AchievementLabel,
                    Description = x.AchievementDescription,
                    Expiration = new Expiration(x.ExpirationType, x.ExpirationFixedDate, x.ExpirationLifetimeQuantity, x.ExpirationLifetimeUnit)
                });
            return list.ToArray();
        }

        private Models.Records.Template GetTemplateItem(Guid id)
        {
            var x = ServiceLocator.AchievementSearch.GetAchievement(id);
            var template = new Models.Records.Template
            {
                Identifier = x.AchievementIdentifier,
                Name = x.AchievementTitle,
                Label = x.AchievementLabel,
                Description = x.AchievementDescription,
                Expiration = new Expiration(x.ExpirationType, x.ExpirationFixedDate, x.ExpirationLifetimeQuantity, x.ExpirationLifetimeUnit)
            };
            return template;
        }
    }
}
