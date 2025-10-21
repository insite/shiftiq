using System.Linq;

using Humanizer;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Events.Reports.Controls
{
    public partial class RecentAppointmentsList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(QEventFilter filter, int count)
        {
            var events = ServiceLocator.EventSearch.GetRecentEvents(filter, count);
            ItemCount = events.Count;

            EventRepeater.DataSource = events.Select(x =>
            {
                return new
                {
                    x.EventType,
                    x.EventIdentifier,
                    x.EventTitle,
                    EventTime = x.EventScheduledStart.Format(User.TimeZone)
                        + $" <span class='form-text'>({x.EventScheduledStart.Humanize()})</span>",
                    LastChangeTimestamp = UserSearch.GetTimestampHtml(x.LastChangeUser, x.LastChangeType, null, x.LastChangeTime)
                };
            });
            EventRepeater.DataBind();
        }
    }
}