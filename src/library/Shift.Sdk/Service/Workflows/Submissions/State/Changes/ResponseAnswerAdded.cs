using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseAnswerAdded : Change
    {
        public ResponseAnswerAdded(Guid question)
        {
            Question = question;
        }

        public Guid Question { get; set; }
    }
}
