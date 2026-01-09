using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.NCSHA;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Custom.NCSHA.History.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<HistoryFilter>
    {
        #region Classes

        private class DataItem
        {
            public Guid RecordID { get; internal set; }

            public Guid UserID { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public string UserGroup { get; set; }

            public DateTimeOffset RecordTime { get; set; }
            public string RecordTimeFormated => RecordTime.Format(User.TimeZone);

            public string EventGroup { get; set; }
            public string EventName { get; set; }
            public string EventData { get; internal set; }

            public string Name { get; set; }
            public string Region { get; set; }
            public string Year { get; set; }
            public string Options { get; set; }
            public string Axis { get; set; }
        }

        #endregion

        #region Binding

        protected override int SelectCount(HistoryFilter filter)
        {
            return HistoryRepository.Count(filter);
        }

        protected override System.ComponentModel.IListSource SelectData(HistoryFilter filter)
        {
            filter.OrderBy = "RecordTime DESC";

            var chartPrograms = ChartModel.GetPrograms(false);
            var eventMetadata = Forms.Search.DeclaredEvents.ToDictionary(x => x.Type.FullName);
            var entities = HistoryRepository.Bind(x => x, filter);
            var data = new List<DataItem>();

            if (entities.Length > 0)
            {
                var users = ServiceLocator.PersonSearch
                    .GetPersons(
                        new QPersonFilter
                        {
                            OrganizationIdentifier = Organization.Identifier,
                            UserIdentifiers = entities.Select(x => x.UserId).Distinct().ToArray()
                        },
                        x => x.EmployerGroup)
                    .Select(x => new { x.UserIdentifier, ParentName = x.EmployerGroup?.GroupName })
                    .ToDictionary(x => x.UserIdentifier);

                foreach (var entity in entities)
                {
                    var item = new DataItem
                    {
                        RecordID = entity.RecordId,
                        UserID = entity.UserId,
                        UserName = entity.UserName,
                        UserEmail = entity.UserEmail,
                        RecordTime = entity.RecordTime,
                        EventData = entity.EventData
                    };

                    if (users.TryGetValue(entity.UserId, out var user))
                        item.UserGroup = user.ParentName;

                    if (eventMetadata.TryGetValue(entity.EventType, out var e))
                    {
                        item.EventGroup = e.Group;
                        item.EventName = e.Name;

                        var eventData = JsonConvert.DeserializeObject(entity.EventData, e.Type);
                        if (eventData is SsrsHistoryEvent ssrsEvent)
                        {
                            item.Name = $"{ssrsEvent.Code}: {ssrsEvent.Name}";
                            item.Year = ssrsEvent.Criteria.Where(x => x.Name == "Year").Select(x => x.Value).FirstOrDefault();
                        }
                        else if (eventData is ChartHistoryEvent chartEvent)
                        {
                            item.Name = string.Empty;
                            item.Region = string.Empty;
                            item.Year = string.Empty;
                            item.Options = string.Empty;
                            item.Axis = string.Empty;

                            if (chartEvent.Criteria.Count == 1)
                            {
                                var criteria = chartEvent.Criteria[0];
                                var fields = criteria.Fields.Select(f => new { Program = chartPrograms.Get(f.Code).Title, Title = f.Name }).Distinct();

                                var isFirstGroup = true;
                                foreach (var group in fields.GroupBy(x => x.Program).OrderBy(x => x.Key))
                                {
                                    if (isFirstGroup)
                                        isFirstGroup = false;
                                    else
                                        item.Name += "; ";

                                    item.Name += $"{group.Key}: ";

                                    var isFirstField = true;
                                    foreach (var field in group.OrderBy(x => x.Title))
                                    {
                                        if (isFirstField)
                                            isFirstField = false;
                                        else
                                            item.Name += ", ";

                                        item.Name += field.Title;
                                    }
                                }

                                if (criteria.Regions.Length > 0)
                                    item.Region = string.Join(", ", criteria.Regions);

                                if (criteria.Year.From.HasValue || criteria.Year.To.HasValue)
                                    item.Year = $"{criteria.Year.From} - {criteria.Year.To}";

                                item.Options = $"{criteria.Func}, {criteria.DatasetType}";
                                item.Axis = criteria.AxisName;
                            }
                        }
                    }
                    else
                    {
                        item.EventGroup = "(Unknown)";
                        item.EventName = "(Unknown)";
                    }

                    data.Add(item);
                }
            }

            return data.ToSearchResult();
        }

        #endregion
    }
}