using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseAnswerChanged : Change
    {
        public ResponseAnswerChanged(Guid question, string answer)
        {
            Question = question;
            Answer = answer;
        }

        public Guid Question { get; set; }
        public string Answer { get; set; }
    }

    public class ResponseGroupChanged : Change
    {
        public ResponseGroupChanged(Guid? group)
        {
            Group = group;
        }

        public Guid? Group { get; set; }
    }

    public class ResponsePeriodChanged : Change
    {
        public ResponsePeriodChanged(Guid? period)
        {
            Period = period;
        }

        public Guid? Period { get; set; }
    }
}
