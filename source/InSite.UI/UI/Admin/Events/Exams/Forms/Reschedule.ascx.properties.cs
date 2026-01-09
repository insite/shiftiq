using System;
using System.Web;
using System.Web.UI;

using InSite.Application.Events.Read;

namespace InSite.Admin.Events.Exams.Forms
{
    public partial class Reschedule
    {
        private class RescheduleProperties
        {
            public RescheduleProperties(StateBag state)
            {
                var request = HttpContext.Current.Request;

                EventIdentifier = Guid.TryParse(request["event"], out var result) ? result : (Guid?)null;

                Event = EventIdentifier.HasValue ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value, x => x.VenueLocation) : null;
            }

            public QEvent Event { get; }

            public Guid? EventIdentifier { get; }

            public string OutlineUrl => $"/ui/admin/events/exams/outline?event={EventIdentifier.Value}";

            public string SearchUrl => $"/ui/admin/events/exams/search";

            public DateTimeOffset CalculateEndTime(DateTimeOffset startDateTime, int duration, string durationUnit)
            {
                var result = startDateTime;

                if (durationUnit == "Minute") result = result.AddMinutes(duration);
                else if (durationUnit == "Hour") result = result.AddHours(duration);
                else if (durationUnit == "Day") result = result.AddDays(duration);
                else if (durationUnit == "Week") result = result.AddDays(7 * duration);
                else if (durationUnit == "Month") result = result.AddMonths(duration);
                else if (durationUnit == "Year") result = result.AddYears(duration);

                return result;
            }
        }
    }
}