using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseOptionSelected : Change
    {
        public ResponseOptionSelected(Guid item)
        {
            Item = item;
        }

        public Guid Item { get; set; }
    }
}
