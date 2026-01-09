using System;
using System.ComponentModel;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Shift.Common.Linq;

namespace InSite.Admin.Events.Exams.Controls
{
    public partial class DependencyDetail : SearchResultsGridViewController<QEventAttendeeFilter>
    {
        protected override bool IsFinder => false;

        public void LoadData(Guid @event)
        {
            Search(new QEventAttendeeFilter
            {
                EventIdentifier = @event
            });
        }

        protected override int SelectCount(QEventAttendeeFilter filter)
        {
            return ServiceLocator.EventSearch.CountAttendees(filter);
        }

        protected override IListSource SelectData(QEventAttendeeFilter filter)
        {
            if (filter == null)
                return null;

            return ServiceLocator.EventSearch
                .GetAttendees(filter, x => x.Person.User)
                .ToSearchResult();
        }
    }
}