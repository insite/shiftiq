using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class AddEnrollment : Command
    {
        public AddEnrollment(Guid gradebook, Guid enrollment, Guid learner, Guid? period, DateTimeOffset? time, string comment)
        {
            AggregateIdentifier = gradebook;

            Enrollment = enrollment;
            Learner = learner;
            Period = period;
            Time = time;
            Comment = comment;
        }

        public Guid Enrollment { get; set; }
        public Guid Learner { get; set; }
        public Guid? Period { get; set; }
        public DateTimeOffset? Time { get; set; }
        public string Comment { get; set; }
    }

    public class RestartEnrollment : Command
    {
        public RestartEnrollment(Guid gradebook, Guid learner, DateTimeOffset? time)
        {
            AggregateIdentifier = gradebook;

            Learner = learner;
            Time = time;
        }

        public Guid Learner { get; set; }
        public DateTimeOffset? Time { get; set; }
    }
}