using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class ExamScheduled2 : Change
    {
        public Guid Tenant { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string BillingCode { get; set; }
        public string ClassCode { get; set; }
        public string Source { get; set; }

        public int Duration { get; set; }
        public int Number { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public ExamScheduled2(Guid tenant, string type, string format, string title, string status, string billingCode, string classCode, string source, int duration, int number, DateTimeOffset start)
        {
            Tenant = tenant;
            Type = type;
            Format = format;
            Title = title;
            Status = status;
            BillingCode = billingCode;
            ClassCode = classCode;
            Source = source;

            Duration = duration;
            Number = number;

            StartTime = start;
        }
    }
}