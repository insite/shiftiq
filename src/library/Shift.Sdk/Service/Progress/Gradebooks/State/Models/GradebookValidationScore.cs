using System;

namespace InSite.Domain.Records
{
    public class GradebookValidationScore
    {
        public Guid User { get; set; }
        public Guid Competency { get; set; }
        public decimal? Points { get; set; }
    }
}
