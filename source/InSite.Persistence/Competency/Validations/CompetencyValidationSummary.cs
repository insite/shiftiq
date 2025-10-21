using System;

namespace InSite.Persistence
{
    public class CompetencyValidationSummary
    {
        public string ChangeComment { get; set; }
        public string ChangeStatus { get; set; }
        public string CompetencyCode { get; set; }
        public string CompetencyTitle { get; set; }
        public string UserName { get; set; }
        public string ValidatorName { get; set; }

        public Guid? AuthorUserIdentifier { get; set; }
        public Guid ChangeIdentifier { get; set; }
        public Guid StandardIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }

        public DateTimeOffset ChangePosted { get; set; }
    }
}
