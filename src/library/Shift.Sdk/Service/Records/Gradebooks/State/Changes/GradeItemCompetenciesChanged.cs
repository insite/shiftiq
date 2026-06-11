using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class GradeItemCompetenciesChanged : Change
    {
        public Guid Item { get; set; }
        public Guid[] Competencies { get; set; }

        public GradeItemCompetenciesChanged(Guid item, Guid[] competencies)
        {
            Item = item;
            Competencies = competencies;
        }
    }
}
