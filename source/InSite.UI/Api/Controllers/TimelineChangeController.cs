using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

using Shift.Common.Timeline.Changes;

using InSite.Api.Settings;
using InSite.Domain.Courses;
using InSite.Persistence;
using InSite.Web.Change;

using Newtonsoft.Json.Linq;

using Shift.Common;

namespace InSite.Api.Controllers
{
    [DisplayName("Timeline")]
    public class ChangesController : ApiBaseController
    {
        private static readonly string[] AdvancedTypes =
        {
            "Progress"
        };

        private static readonly string[] ExcludedProperties = new[]
        {
            "AggregateIdentifier",
            "AggregateVersion",
            "ChangeTime",
            "ChangeType",
            "ChangeName",
            "ChangeData",
            "OriginOrganization",
            "OriginUser"
        };

        private static readonly Dictionary<string, BaseChangeDescriptor> _descriptors = new Dictionary<string, BaseChangeDescriptor>
        {
            { ChangeStore.GetAggregateType(typeof(CourseAggregate)), new CourseChangeDescriptor(ExcludedProperties) }
        };

        private static readonly BaseChangeDescriptor _defaultDescriptor = new BaseChangeDescriptor(ExcludedProperties, null);

        [HttpGet]
        [Route("api/changes/get-data")]
        public HttpResponseMessage GetData(Guid aggregate, int version)
        {
            var aggregateData = ServiceLocator.AggregateSearch.Get(aggregate);
            if (aggregateData == null)
                return JsonError("NOT FOUND", HttpStatusCode.NotFound);

            var json = GetDescription(aggregateData, version);
            if (string.IsNullOrEmpty(json))
                return JsonError("NOT FOUND", HttpStatusCode.NotFound);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        private static string GetDescription(SerializedAggregate aggregate, int version)
        {
            if (string.Equals(aggregate.AggregateType, "Case", StringComparison.OrdinalIgnoreCase))
                return new CaseChangeDescriptor(aggregate.AggregateIdentifier, version, ExcludedProperties).GetDescription();

            if (!_descriptors.TryGetValue(aggregate.AggregateType, out var descriptor))
                descriptor = _defaultDescriptor;

            var json = descriptor.GetDescription(aggregate.AggregateIdentifier, version);
            if (json == null || !StringHelper.EqualsAny(aggregate.AggregateType, AdvancedTypes))
                return json;

            var changeProps = JObject.Parse(json);

            AddExtraProperties(aggregate, changeProps);
            AddChangedProperties(aggregate, version, changeProps);

            return changeProps.ToString();
        }

        private static void AddExtraProperties(SerializedAggregate aggregate, JObject changeProps)
        {
            if (!string.Equals(aggregate.AggregateType, "Progress"))
                return;

            // Only for Progress, improve the code when more aggregates are added

            var progress = ServiceLocator.RecordSearch.GetProgress(aggregate.AggregateIdentifier, x => x.GradeItem, x => x.Learner);
            if (progress == null)
                return;

            changeProps.AddFirst(new JProperty("Learner", progress.Learner?.UserFullName));
            changeProps.AddFirst(new JProperty("Grade Item Name", progress.GradeItem?.GradeItemName));
            changeProps.AddFirst(new JProperty("Grade Item Identifier", progress.GradeItemIdentifier));
        }

        private static void AddChangedProperties(SerializedAggregate aggregate, int version, JObject changeProps)
        {
            var (prev, current) = ServiceLocator.SnapshotRepository.GetPrevAndCurrentStates(aggregate.AggregateIdentifier, version);
            if (prev == null)
                return;

            var differences = ObjectComparer.Compare(prev, current, ExcludedProperties);
            if (differences.Count == 0)
                return;

            var extraProps = new JObject();
            foreach (var diff in differences)
            {
                extraProps.Add(new JProperty($"Old {diff.PropertyName}", diff.ValueBefore));
                extraProps.Add(new JProperty($"New {diff.PropertyName}", diff.ValueAfter));
            }

            changeProps.Add(new JProperty("Changed Properties", extraProps));
        }
    }
}