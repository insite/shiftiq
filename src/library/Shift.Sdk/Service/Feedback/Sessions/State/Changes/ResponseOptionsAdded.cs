using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseOptionsAdded : Change
    {
        public ResponseOptionsAdded(Guid question, Guid[] items)
        {
            Question = question;
            Items = items;
        }

        public Guid Question { get; set; }
        public Guid[] Items { get; set; }
    }
}
