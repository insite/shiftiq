using System;
using System.ComponentModel;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Records.Outcomes.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QGradebookCompetencyValidationFilter>
    {
        protected override int SelectCount(QGradebookCompetencyValidationFilter filter)
        {
            return ServiceLocator.RecordSearch.CountValidations(filter);
        }

        protected override IListSource SelectData(QGradebookCompetencyValidationFilter filter)
        {
            return ServiceLocator
                .RecordSearch
                .GetValidations(
                    filter,
                    x => x.Gradebook.Event, x => x.Gradebook.Achievement, x => x.Student, x => x.Standard)
                .ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.FormatDateOnly(User.TimeZone, nullValue: string.Empty);
        }
    }
}