using System;

using InSite.Application.Standards.Read;

namespace InSite.Application.Records.Read
{
    public class QGradeItemCompetency
    {
        public Guid CompetencyIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid GradeItemIdentifier { get; set; }

        public virtual QGradebook Gradebook { get; set; }
        public virtual QGradeItem GradeItem { get; set; }
        public virtual VStandard Standard { get; set; }
    }
}
