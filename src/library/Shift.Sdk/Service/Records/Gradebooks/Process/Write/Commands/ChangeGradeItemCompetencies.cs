using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Gradebooks.Write
{
    public class ChangeGradeItemCompetencies : Command
    {
        public Guid Item { get; set; }
        public Guid[] Competencies { get; set; }

        public ChangeGradeItemCompetencies(Guid record, Guid item, Guid[] competencies)
        {
            AggregateIdentifier = record;
            Item = item;
            Competencies = competencies;
        }
    }
}
