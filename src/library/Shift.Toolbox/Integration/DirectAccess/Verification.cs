using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    [Serializable]
    public class Verification
    {
        public VerificationInputVariables InputVariables { get; set; }
        public VerificationDisplayVariables DisplayVariables { get; set; }

        public List<string> Errors { get; set; }
        public List<string> Warnings { get; set; }

        #region Properties (inferred values)

        [JsonIgnore]
        public int EventCountdownDays => (int)Math.Round((InputVariables.EventTime - InputVariables.CurrentTime).TotalDays, 0);

        [JsonIgnore]
        public int EventCountdownHours => (InputVariables.EventTime - InputVariables.CurrentTime).Hours;

        [JsonIgnore]
        public bool AdminIsInternal => true;

        [JsonIgnore]
        public bool EventIsPaper => !InputVariables.EventIsOnline;

        [JsonIgnore]
        public bool EventTimeIsHoliday => IsHoliday(InputVariables.EventTime);

        private bool IsHoliday(DateTimeOffset time)
        {
            return InputVariables.HolidayCalendar.Any(holiday => holiday.Date == time.Date);
        }

        [JsonIgnore]
        public bool EventTimeIsWeekend
        {
            get
            {
                var day = InputVariables.EventTime.Date.DayOfWeek;
                return day == DayOfWeek.Saturday || day == DayOfWeek.Sunday;
            }
        }

        #endregion

        public Verification(VerificationInputVariables input, VerificationDisplayVariables display)
        {
            InputVariables = input;
            DisplayVariables = display;
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        public void Calculate()
        {
            CalculateErrors();
            CalculateWarnings();
        }

        private void CalculateErrors()
        {
            if (InputVariables.ClassStatus == "Not Verified")
                Errors.Add($"Class/session {DisplayVariables.ClassCode} is not a valid identifier in DA. (Direct Access: {DisplayVariables.ClassStatusReason})");

            if (InputVariables.ClassFormCodes.Count > 0 && DisplayVariables.ClassCode != null)
                if (!InputVariables.ClassFormCodes.Contains(InputVariables.FormCode, StringComparer.OrdinalIgnoreCase))
                    Errors.Add($"Class/session {DisplayVariables.ClassCode} does not contain exam {InputVariables.FormCode}.");

            if (EventCountdownDays > 90)
                Errors.Add($"{DisplayVariables.CandidateName} cannot be registered for an exam after {Format(InputVariables.CurrentTime.AddDays(90))} (more than 90 days from now).");

            if (EventCountdownDays < 14 && EventIsPaper)
                Errors.Add($"{DisplayVariables.CandidateName} cannot be registered for this paper exam after {FormatDateOnly(InputVariables.EventTime.AddDays(-14))}, which is the 14-day cut-off before the exam starts.");

            if (EventCountdownDays <= 3 && InputVariables.EventIsOnline && !AdminIsInternal)
                Errors.Add($"{DisplayVariables.CandidateName} cannot be registered for this external online exam before {Format(InputVariables.CurrentTime.AddDays(3))} (less than 3 days from now).");

            if (EventCountdownHours < -1 && InputVariables.EventIsOnline && AdminIsInternal)
                Errors.Add($"{DisplayVariables.CandidateName} cannot be registered for this internal online exam after {Format(InputVariables.EventTime.AddHours(1))} (more than 1 hour after the exam has started).");

            if (InputVariables.TradeLevelAttempted.HasValue)
                if (InputVariables.EventTime <= InputVariables.TradeLevelAttempted.Value.AddDays(30))
                    Errors.Add($"{DisplayVariables.CandidateName} registered for a previous exam in the same program and level {Format(InputVariables.TradeLevelAttempted.Value)} (less than 30 days ago).");

            if (InputVariables.CandidateStatus == "Not Verified")
                Errors.Add($"{DisplayVariables.CandidateName} is not an active individual in DA. (Direct Access: {DisplayVariables.CandidateStatusReason})");

            if (InputVariables.ScheduleConflictCount > 0)
                Errors.Add($"{DisplayVariables.CandidateName} is registered for another exam on the same day at the same time.");
        }

        private string FormatDateOnly(DateTimeOffset time)
        {
            return $"{time:MMM d, yyyy}";
        }

        private string Format(DateTimeOffset time)
        {
            return $"{time:MMM d, yyyy} {time:h:mm tt}";
        }

        private void CalculateWarnings()
        {
            if (EventTimeIsWeekend)
                Warnings.Add($"This {DisplayVariables.EventType} exam event is scheduled on a weekend.");

            if (EventTimeIsHoliday)
                Warnings.Add($"This {DisplayVariables.EventType} exam event is scheduled on a holiday.");

            if (InputVariables.FormConflictCount > 0)
                Warnings.Add($"{DisplayVariables.CandidateName} is registered for the same exam form within 30 days.");

            if (InputVariables.TradeStatus == "Not Verified")
                Warnings.Add($"{DisplayVariables.CandidateName} is not registered in a program that matches the trade for exam {InputVariables.FormCode}. (Direct Access: {DisplayVariables.TradeStatusReason})");
        }
    }
}