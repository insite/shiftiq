using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Common.Web.UI;

using Shift.Common;

using CheckBoxList = System.Web.UI.WebControls.CheckBoxList;

namespace InSite.Admin.Logs.Commands.Controls
{
    public partial class RecurrenceField : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RecurrenceValidator.ServerValidate += RecurrenceValidator_ServerValidate;
        }

        private void RecurrenceValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var validator = (BaseValidator)source;
            var hasInterval = RecurrenceInterval.ValueAsInt.HasValue && RecurrenceInterval.ValueAsInt.Value > 0;
            var hasUnit = !string.IsNullOrEmpty(RecurrenceUnit.Value);

            args.IsValid = hasInterval == hasUnit;

            if (args.IsValid)
                validator.ErrorMessage = string.Empty;
            else if (hasInterval)
                validator.ErrorMessage = "Required field: Recurrence Unit.";
            else if (hasUnit)
                validator.ErrorMessage = "Required field: Recurrence Interval.";
        }

        public void SetInputValues(SerializedCommand command)
        {
            RecurrenceInterval.ValueAsInt = command.RecurrenceInterval;
            RecurrenceUnit.Value = command.RecurrenceUnit;
            SetRecurrenceWeekDays(RecurrenceWeekdays, command.RecurrenceWeekdays);

            if (command.RecurrenceInterval.HasValue && command.RecurrenceUnit != null && command.SendScheduled.HasValue)
            {
                var now = DateTimeOffset.UtcNow;
                var last = Shift.Common.Calendar.CalculateLastInterval(command.SendScheduled.Value, now, command.RecurrenceUnit, command.RecurrenceInterval.Value);
                var next = Shift.Common.Calendar.CalculateNextInterval(command.SendScheduled.Value, now, command.RecurrenceUnit, command.RecurrenceInterval.Value);

                if (last < now && last != command.SendScheduled)
                    LastInterval.Text = "<div>Last reccurrence " + TimeZones.Format(last, User.TimeZone, true) + "</div>";

                if (next != command.SendScheduled)
                    NextInterval.Text = "<div>Next recurrence " + TimeZones.Format(next, User.TimeZone, true) + "</div>";
            }
        }

        private void SetRecurrenceWeekDays(CheckBoxList checklist, string weekdays)
        {
            checklist.ClearSelection();
            if (weekdays != null)
            {
                var days = weekdays.Split(new char[] { ',' });
                foreach (var day in days)
                    checklist.Items.FindByValue(day).Selected = true;
            }
        }

        public void GetInputValues(SerializedCommand command)
        {
            var hasInterval = RecurrenceInterval.ValueAsInt.HasValue && RecurrenceInterval.ValueAsInt.Value > 0;
            var unit = RecurrenceUnit.Value;

            if (hasInterval && unit.IsNotEmpty())
            {
                command.RecurrenceInterval = RecurrenceInterval.ValueAsInt;
                command.RecurrenceUnit = unit;
                command.RecurrenceWeekdays = GetRecurrenceWeekDays(RecurrenceWeekdays);
            }
            else
            {
                command.RecurrenceInterval = null;
                command.RecurrenceUnit = null;
                command.RecurrenceWeekdays = null;
            }
        }

        private string GetRecurrenceWeekDays(CheckBoxList checklist)
        {
            var list = new List<String>();
            foreach (System.Web.UI.WebControls.ListItem day in checklist.Items)
                if (day.Selected)
                    list.Add(day.Value);
            if (list.Count > 0)
                return string.Join(",", list);
            return null;
        }
    }
}