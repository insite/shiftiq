using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Standards.Read;

namespace InSite.Application.Records.Read
{
    public class QGradebookCompetencyValidation
    {
        public Guid ValidationIdentifier { get; set; }

        public Guid CompetencyIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public decimal? ValidationPoints { get; set; }

        public virtual QGradebook Gradebook { get; set; }
        public virtual VUser Student { get; set; }
        public virtual VStandard Standard { get; set; }
    }
}
