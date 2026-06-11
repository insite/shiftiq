using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradebookValidationChanged : Change
    {
        public GradebookValidationChanged(Guid user, Guid competency, decimal? points)
        {
            User = user;
            Competency = competency;
            Points = points;
        }

        public Guid User { get; set; }
        public Guid Competency { get; set; }
        public decimal? Points { get; set; }
    }
}