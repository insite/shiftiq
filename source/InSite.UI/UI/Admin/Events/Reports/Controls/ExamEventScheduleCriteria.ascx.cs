using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Events.Reports.Controls
{
    public partial class ExamEventScheduleCriteria : SearchCriteriaController<VExamEventScheduleFilter>
    {
        public override VExamEventScheduleFilter Filter
        {
            get
            {
                var filter = new VExamEventScheduleFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    ScheduledSince = ScheduledSince.Value?.UtcDateTime,
                    ScheduledBefore = ScheduledBefore.Value?.UtcDateTime
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ScheduledSince.Value = value.ScheduledSince;
                ScheduledBefore.Value = value.ScheduledBefore;
            }
        }

        public override void Clear()
        {
            ScheduledSince.Value = null;
            ScheduledBefore.Value = null;

            CustomDateRange.ClearSelection();
            OnCustomDateRangeChanged(null, ScheduledSince, ScheduledBefore);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CustomDateRange.AutoPostBack = true;
            CustomDateRange.ValueChanged += CustomDateRange_ValueChanged;
        }

        private void CustomDateRange_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            OnCustomDateRangeChanged(e.NewValue, ScheduledSince, ScheduledBefore);
        }

        private void OnCustomDateRangeChanged(string value, DateTimeOffsetSelector selectorFrom, DateTimeOffsetSelector selectorThru)
        {
            if (value.HasValue())
            {
                if (value.Contains("Next"))
                {
                    var start = DateTimeOffset.UtcNow.AddDays(1);

                    if (value == "Next 30 Days")
                    {
                        selectorFrom.Value = start;
                        selectorThru.Value = start.AddDays(30);
                    }
                    else if (value == "Next 60 Days")
                    {
                        selectorFrom.Value = start;
                        selectorThru.Value = start.AddDays(60);
                    }
                    else if (value == "Next 90 Days")
                    {
                        selectorFrom.Value = start;
                        selectorThru.Value = start.AddDays(90);
                    }
                }
            }

            selectorFrom.Enabled = selectorThru.Enabled = !value.HasValue();
        }
    }
}