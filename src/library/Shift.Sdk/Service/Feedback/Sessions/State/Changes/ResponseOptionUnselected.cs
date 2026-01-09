using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseOptionUnselected : Change
    {
        public ResponseOptionUnselected(Guid item)
        {
            Item = item;
        }

        public Guid Item { get; set; }
    }
}
