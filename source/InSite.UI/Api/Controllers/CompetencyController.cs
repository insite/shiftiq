using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Competency")]
    [RoutePrefix("api/standards")]
    public partial class StandardsController : ApiBaseController
    {
        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _treeStateLocks = 
            new ConcurrentDictionary<Guid, SemaphoreSlim>();

        [HttpGet]
        [Route("list-competencies")]
        public HttpResponseMessage List()
        {
            var request = HttpContext.Current.Request;

            var model = request["model"];
            var subtype = request["subtype"];
            var keyword = request["keyword"];
            var isPublished = request["published"];

            var filter = new StandardFilter
            {
                OrganizationIdentifier = CurrentOrganization.OrganizationIdentifier
            };

            if (!string.IsNullOrEmpty(subtype))
                filter.StandardTypes = new[] { subtype };

            if (!string.IsNullOrEmpty(keyword))
                filter.SelectorText = keyword;

            if (!string.IsNullOrEmpty(isPublished))
                filter.IsPublished = isPublished == "1";

            object result;

            if (model == "2")
            {
                var startRow = int.Parse(request["start"]);
                var endRow = int.Parse(request["end"]);

                filter.Paging = Paging.SetStartEnd(startRow, endRow);
                filter.OrderBy = "ContentTitle";

                result = StandardSearch.Bind(
                    x => new JsonOptionModel2
                    {
                        Id = x.StandardIdentifier,
                        Subtype = x.StandardType,
                        Number = x.AssetNumber,
                        Name = x.ContentName,
                        Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title),
                        ContentTitle = x.ContentTitle,

                        ParentSubtype = x.Parent.StandardType,
                        ParentNumber = x.Parent.AssetNumber,
                        ParentTitle = CoreFunctions.GetContentTextEn(x.Parent.StandardIdentifier, ContentLabel.Title),
                        Code = x.Code
                    },
                    filter);
            }
            else
            {
                filter.Paging = Paging.SetSkipTake(0, 20);
                filter.OrderBy = "Number";

                result = StandardSearch.Bind(
                    x => new JsonOptionModel1
                    {
                        Number = x.AssetNumber,
                        Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title),
                        Code = x.Code
                    },
                    filter);
            }

            return JsonSuccess(result);
        }

        [HttpPost]
        [Route("treestate")]
        public async Task<IHttpActionResult> TreeState(Guid program, [FromBody] TreeStateParams parameters)
        {
            if (parameters == null)
                return BadRequest();

            var hasCollapsed = parameters.Collapsed != null;
            var hasExpanded = parameters.Expanded != null;

            if (!hasCollapsed && !hasExpanded)
                return BadRequest();

            HashSet<Guid> validNumbers;
            {
                var filter = new List<Guid>();

                filter.Add(program);

                if (hasCollapsed && parameters.Collapsed.Length > 0)
                    filter.AddRange(parameters.Collapsed);

                if (hasExpanded && parameters.Expanded.Length > 0)
                    filter.AddRange(parameters.Expanded);

                validNumbers = StandardSearch
                    .Bind(
                        x => x.StandardIdentifier,
                        x => x.OrganizationIdentifier == CurrentOrganization.OrganizationIdentifier && filter.Contains(x.StandardIdentifier))
                    .ToHashSet();
            }

            if (!validNumbers.Contains(program))
                return NotFound();

            var userId = CurrentUser.UserIdentifier;
            var @lock = _treeStateLocks.GetOrAdd(userId, (key) => new SemaphoreSlim(1, 1));

            await @lock.WaitAsync();

            try
            {
                var name = PersonalizationName.AssetOutlineTreeState + $".{program}";
                var data = PersonalizationRepository.GetValue<HashSet<Guid>>(Guid.Empty, userId, name, false)
                    ?? new HashSet<Guid>();

                if (hasCollapsed)
                    foreach (var node in parameters.Collapsed)
                    {
                        if (!validNumbers.Contains(node))
                            continue;

                        if (data.Contains(node))
                            data.Remove(node);
                    }

                if (hasExpanded)
                    foreach (var node in parameters.Expanded)
                    {
                        if (!validNumbers.Contains(node))
                            continue;

                        if (!data.Contains(node))
                            data.Add(node);
                    }

                PersonalizationRepository.SetValue(Guid.Empty, userId, name, data);

                return Ok();
            }
            finally
            {
                @lock.Release();
            }
        }
    }
}