using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Gradebooks.Controls
{
    public partial class AssignPeriodGrid : SearchResultsGridViewController<QEnrollmentFilter>
    {
        protected override bool IsFinder => false;

        public int LoadData(
            Guid gradebookIdentifier,
            Guid? periodIdentifier,
            string searchKeyword,
            bool hideAssignedPeriod,
            DateTimeOffset? gradedSince,
            DateTimeOffset? gradedBefore
            )
        {
            var filter = new QEnrollmentFilter
            {
                GradebookIdentifier = gradebookIdentifier,
                PeriodIdentifier = periodIdentifier,
                SearchKeyword = searchKeyword,
                IsPeriodAssigned = hideAssignedPeriod ? false : (bool?)null,
                GradedSince = gradedSince,
                GradedBefore = gradedBefore
            };

            Search(filter);

            return RowCount;
        }

        public List<Guid> GetUsers()
        {
            var result = new List<Guid>();

            for (int i = 0; i < Grid.DataKeys.Count; i++)
                result.Add(Grid.GetDataKey<Guid>(i));

            return result;
        }

        protected override int SelectCount(QEnrollmentFilter filter)
        {
            return ServiceLocator.RecordSearch.CountEnrollments(filter);
        }

        protected override IListSource SelectData(QEnrollmentFilter filter)
        {
            return ServiceLocator.RecordSearch
                .GetEnrollmentsForPeriodGrid(filter)
                .ToSearchResult();
        }

        protected string GetLocalDate(object item)
        {
            if (item == null)
                return string.Empty;

            var when = (DateTimeOffset?)item;
            var converted = TimeZoneInfo.ConvertTime(when.Value, User.TimeZone);

            return $"{converted:MMM d, yyyy}";
        }
    }
}