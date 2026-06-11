using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradebookValidation : Command
    {
        public ChangeGradebookValidation(Guid record, Guid user, Guid competency, decimal? points)
        {
            AggregateIdentifier = record;
            User = user;
            Competency = competency;
            Points = points;
        }

        public Guid User { get; set; }
        public Guid Competency { get; set; }
        public decimal? Points { get; set; }
    }
}