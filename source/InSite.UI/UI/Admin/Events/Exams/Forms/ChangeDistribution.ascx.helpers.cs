using System;

using InSite.Application.Events.Read;
using InSite.Common.Web;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class ChangeDistribution
    {
        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var result) ? result : (Guid?)null;

        private string OutlineUrl
            => $"/ui/admin/events/exams/outline?event={EventIdentifier.Value}&panel=event.distribution";

        private string SearchUrl
            => $"/ui/admin/events/exams/search";

        private void NavigateToSearch()
            => HttpResponseHelper.Redirect(SearchUrl, true);

        private QEvent GetEvent() => EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value) : null;
    }
}