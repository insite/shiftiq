using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Integration.Moodle;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class ScormEventGrid : SearchResultsGridViewController<ScormEventFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(Guid gradebook)
        {
            var activities = ServiceLocator.RecordSearch.BindGradeItems(
                i => i.Activities.Select(a => a.ActivityIdentifier),
                i => i.GradebookIdentifier == gradebook)
                .SelectMany(x => x)
                .ToArray();

            var filter = new ScormEventFilter
            {
                ActivityIdentifiers = activities
            };

            Search(filter);
        }

        protected override int SelectCount(ScormEventFilter filter)
        {
            var search = new MoodleSearch();
            var count = search.CountMoodleEvents(filter.ActivityIdentifiers);
            return count;
        }

        protected override IListSource SelectData(ScormEventFilter filter)
        {
            var search = new MoodleSearch();
            var list = search.GetMoodleEvents(filter.ActivityIdentifiers)
                .OrderByDescending(x => x.EventWhen)
                .ApplyPaging(filter)
                .ToList();
            return list.ToSearchResult();
        }
    }
}