using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindActivity : BaseFindEntity<FindActivity.DataFilter>
    {
        #region Classes

        public class DataFilter : Filter
        {
            public Guid OrganizationIdentifier { get; set; }
            public string Keyword { get; set; }
        }

        #endregion

        protected override string GetEntityName() => "Activity";

        protected override DataFilter GetFilter(string keyword) => new DataFilter
        {
            OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
            Keyword = keyword
        };

        protected override int Count(DataFilter filter)
        {
            return Persistence.CourseSearch.CountActivities(
                x => x.Module.Unit.Course.OrganizationIdentifier == filter.OrganizationIdentifier && x.ActivityName.StartsWith(filter.Keyword)
            );
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            return Persistence.CourseSearch.BindActivities(
                x => new DataItem { Value = x.ActivityIdentifier, Text = x.ActivityName },
                x => x.Module.Unit.Course.OrganizationIdentifier == filter.OrganizationIdentifier && x.ActivityName.StartsWith(filter.Keyword),
                nameof(DataItem.Text));
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return Persistence.CourseSearch.BindActivities(
                x => new DataItem { Value = x.ActivityIdentifier, Text = x.ActivityName },
                x => ids.Contains(x.ActivityIdentifier),
                nameof(DataItem.Text));
        }
    }
}