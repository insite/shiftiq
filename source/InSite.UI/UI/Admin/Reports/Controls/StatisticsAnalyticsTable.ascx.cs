using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Events.Reports.Controls
{
    public partial class StatisticsAnalyticsTable : BaseUserControl
    {
        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GroupRepeater.ItemDataBound += GroupRepeater_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void GroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var groupData = (EventStatisticGroup)e.Item.DataItem;
            var hasData = groupData.Items.Any();

            var itemsRepeater = (Repeater)e.Item.FindControl("TableRepeater");
            var noDataMessage = e.Item.FindControl("NoDataMessage");

            itemsRepeater.Visible = hasData;
            itemsRepeater.DataSource = groupData.Items;
            itemsRepeater.DataBind();

            noDataMessage.Visible = !hasData;
        }

        #endregion

        #region Methods (data binding)

        public bool LoadData(IEnumerable<QEvent> events, IEnumerable<QRegistration> registrations)
        {
            var groups = new List<EventStatisticGroup>();

            { // Scheduling Status
                var items = events
                    .GroupBy(x => x.EventSchedulingStatus.NullIfEmpty())
                    .Select(x => new EventStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .ToArray();

                groups.Add(new EventStatisticGroup
                {
                    Name = "Scheduling",
                    Items = items.OrderBy(x => x.Text)
                });
            }

            { // Activity Format
                var items = events
                    .GroupBy(x => x.EventFormat.NullIfEmpty())
                    .Select(x => new EventStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                groups.Add(new EventStatisticGroup
                {
                    Name = "Format",
                    Items = items
                });
            }

            { // Exam Type
                var items = events
                    .GroupBy(x => x.ExamType.NullIfEmpty())
                    .Select(x => new EventStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                groups.Add(new EventStatisticGroup
                {
                    Name = "Exam Type",
                    Items = items
                });
            }

            { // Billing Code
                var items = events
                    .GroupBy(x => x.EventBillingType.NullIfEmpty())
                    .Select(x => new EventStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                groups.Add(new EventStatisticGroup
                {
                    Name = "Billing Code",
                    Items = items
                });
            }

            { // Invigilating Office
                var items = events
                    .GroupBy(x => x.VenueLocation?.GroupOffice.NullIfEmpty())
                    .Select(x => new EventStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .OrderBy(x => x.Text)
                    .ToArray();

                groups.Add(new EventStatisticGroup
                {
                    Name = "Invigilating Office",
                    Items = items
                });
            }

            { // Attendance Status
                var items = registrations
                    .GroupBy(x => x.AttendanceStatus.NullIfEmpty())
                    .Select(x => new EventStatisticItem
                    {
                        Text = x.Key,
                        Count = x.Count()
                    })
                    .ToArray();

                groups.Add(new EventStatisticGroup
                {
                    Name = "Attendance",
                    Items = items.OrderBy(x => x.Text)
                });
            }

            GroupRepeater.DataSource = groups;
            GroupRepeater.DataBind();

            return groups.Any(x => x.Items.Any(y => y.Count > 0));
        }

        #endregion
    }
}